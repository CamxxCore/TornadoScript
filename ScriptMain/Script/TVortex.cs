using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GTA;
using GTA.Math;
using GTA.Native;
using TornadoScript.ScriptCore.Game;
using TornadoScript.ScriptMain.Utility;

namespace TornadoScript.ScriptMain.Script
{
    public class TornadoVortex : ScriptExtension
    {
        /// <summary>
        /// Scale of the vortex forces.
        /// </summary>
        public float ForceScale { get; } = 4.0f;

        /// <summary>
        /// Maximum distance entites must be from the vortex before we start using internal vortext forces on them.
        /// </summary>
        public float InternalForcesDist { get; } = 5.0f;

        readonly List<TornadoParticle> _particles = new List<TornadoParticle>();

        readonly List<GameSound> _loadedSounds = new List<GameSound>();

        private int _nextUpdateTime;

        private struct ActiveEntity
        {
            public ActiveEntity(Entity entity, float xBias, float yBias)
            {
                Entity = entity;
                XBias = xBias;
                YBias = yBias;
                IsPlayer = entity == Helpers.GetLocalPed();
            }

            public Entity Entity { get; }
            public float XBias { get; }
            public float YBias { get; }
            public bool IsPlayer { get; }
        }

        public const int MaxEntityCount = 600;

        private readonly ActiveEntity[] _activeEntities =
            new ActiveEntity[MaxEntityCount];

        private int _activeEntityCount;

        private Vector3 _position, _destination;

        public Vector3 Position => _position;

        private readonly Ped _player = Helpers.GetLocalPed();

        private int _lastPlayerShapeTestTime;

        bool _lastRaycastResultFailed;

        public TornadoVortex(Vector3 initialPosition)
        {
            _position = initialPosition;
            _destination = Helpers.GetRandomPositionFromCoords(initialPosition, 10.0f);
        }

        public void Build()
        {
            var layerSize = ScriptThread.GetVar<float>("vortexLayerSeperationScale").Value;

            var radius = ScriptThread.GetVar<float>("vortexRadius").Value;

            var particleCount = ScriptThread.GetVar<int>("vortexParticleCount").Value;

            var maxLayers = ScriptThread.GetVar<int>("vortexMaxParticleLayers");

            var particleAsset = ScriptThread.GetVar<string>("vortexParticleAsset");

            var particleName = ScriptThread.GetVar<string>("vortexParticleName");

            var multiplier = 359 / particleCount;

            var particleSize = 3.0f;

            for (var i = 0; i < maxLayers; i++)
            {
                for (var angle = 0; angle <= particleCount; angle++)
                {
                    // increment the Z axis as we build up.
                    var position = _position;

                    position.Z += layerSize * i;

                    // place the particles at 360 / 10 on the X axis.
                    var rotation = new Vector3(angle * multiplier, 0, 0);

                    TornadoParticle particle;

                    if (i < 2 || i > maxLayers - 3)
                    {
                        particle = new TornadoParticle(this, position, rotation, "scr_agencyheistb",
                            "scr_env_agency3b_smoke", radius, i);

                        particle.StartFx(4.7f);

                        _particles.Add(particle);
                    }

                    particle = new TornadoParticle(this, position, rotation, particleAsset, particleName, radius, i);

                    particle.StartFx(particleSize);

                    radius += 0.0799999982118607f * (0.720000028610229f * i);

                    particleSize += 0.00999999977648258f * (0.119999997317791f * i);

                    _particles.Add(particle);
                }


            }
        }

        private void RemoveEntity(int entityIdx)
        {
            _activeEntityCount -= 1;

            for (var i = entityIdx; i < _activeEntities.Length - 1; i++)
            {
                _activeEntities[i] = _activeEntities[i + 1];
            }
        }

        private void PushBackEntity(ActiveEntity entity)
        {
            for (var i = _activeEntities.Length - 1; i > 0; i--)
            {
                _activeEntities[i] = _activeEntities[i - 1];
            }

            _activeEntities[0] = entity;

            _activeEntityCount = Math.Min(_activeEntityCount + 1, _activeEntities.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CollectNearbyEntities(float maxDistanceDelta)
        {
            foreach (var ent in World.GetAllEntities())
            {
                var entityPos = ent.Position;

                if (!(Vector2.Distance(entityPos.Vec2(), _position.Vec2()) < maxDistanceDelta) ||
                    !(ent.HeightAboveGround < 300.0f)) continue;

                var entityExistsLocally = false;

                for (var x = 0; x < _activeEntityCount; x++)
                {
                    if (_activeEntities[x].Entity != ent) continue;

                    entityExistsLocally = true;

                    break;
                }

                if (entityExistsLocally) continue;

                if (ent is Ped && /*entities[p].Handle != _player.Handle &&*/ !(ent as Ped).IsRagdoll)
                {
                    Function.Call(Hash.SET_PED_TO_RAGDOLL, ent.Handle, 800, 1500, 2, 1, 1, 0);
                }

                PushBackEntity(new ActiveEntity(ent,
                    3.0f * Probability.GetScalar(),
                    3.0f * Probability.GetScalar()));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdatePulledEntities(int gameTime, float maxDistanceDelta)
        {
            float verticalForce = ScriptThread.GetVar<float>("vortexVerticalPullForce");

            float horizontalForce = ScriptThread.GetVar<float>("vortexHorizontalPullForce");

            float topSpeed = ScriptThread.GetVar<float>("vortexTopEntitySpeed");

            for (var e = 0; e < _activeEntityCount; e++)
            {
                var entity = _activeEntities[e].Entity;

                var dist = Vector2.Distance(entity.Position.Vec2(), _position.Vec2());

                if (dist > maxDistanceDelta - 12.6f || entity.HeightAboveGround > 300.0f)
                {
                    RemoveEntity(e);
                    continue;
                }

                var targetPos = new Vector3(_position.X + _activeEntities[e].XBias,
                    _position.Y + _activeEntities[e].YBias, entity.Position.Z);

                var direction = Vector3.Normalize(targetPos - entity.Position);

                var forceBias = Probability.NextFloat();

                var force = ForceScale * (forceBias + forceBias / dist);

                if (_activeEntities[e].IsPlayer)
                {
                    if (gameTime - _lastPlayerShapeTestTime > 1000)
                    {
                        var raycast = World.Raycast(entity.Position, targetPos, IntersectOptions.Map);

                        _lastRaycastResultFailed = raycast.DitHitAnything;

                        _lastPlayerShapeTestTime = gameTime;
                    }

                    if (_lastRaycastResultFailed)
                        continue;

                    verticalForce *= 1.45f;
                    horizontalForce *= 1.5f;
                }

                if (entity.Model.IsPlane)
                {
                    force *= 6.0f;
                    verticalForce *= 6.0f;
                }

                // apply a directional force pulling them into the tornado...
                entity.ApplyForce(direction * horizontalForce,
                    new Vector3(Probability.NextFloat(), 0,
                        Probability.GetScalar()));

                var upDir = Vector3.Normalize(new Vector3(_position.X, _position.Y, _position.Z + 1000.0f) -
                                              entity.Position);
                // apply vertical forces
                entity.ApplyForceToCenterOfMass(upDir * verticalForce);

                var cross = Vector3.Cross(direction, Vector3.WorldUp);

                // move them along side the vortex.
                entity.ApplyForceToCenterOfMass(Vector3.Normalize(cross) * force *
                                                horizontalForce);

                Function.Call(Hash.SET_ENTITY_MAX_SPEED, entity.Handle, topSpeed);
            }
        }

        public override void OnUpdate(int gameTime)
        {
            if (ScriptThread.GetVar<bool>("vortexMovementEnabled"))
            {
                if (_position.DistanceTo(_player.Position) > 200.0f)
                {
                    _destination = _player.Position.Around(50.0f);
                    _destination.Z = World.GetGroundHeight(_destination) - 10.0f;
                }

                else if (_position.DistanceTo(_destination) < 15.0f)
                {
                    _destination = Helpers.GetRandomPositionFromCoords(_position, 10.0f);
                    _destination.Z = World.GetGroundHeight(_destination) - 10.0f;
                }

                _position = MathEx.MoveTowards(_position, _destination,
                    ScriptThread.GetVar<float>("vortexMoveSpeedScale") * 0.287f);
            }

            float maxEntityDist = ScriptThread.GetVar<float>("vortexMaxEntityDist");

            if (gameTime > _nextUpdateTime)
            {
                CollectNearbyEntities(maxEntityDist);

                _nextUpdateTime = gameTime + 800;
            }

            UpdatePulledEntities(gameTime, maxEntityDist);
        }

        public override void Dispose()
        {
            _loadedSounds.ForEach(x => x.Destroy());

            _particles.ForEach(x => x.Dispose());

            base.Dispose();
        }
    }
}
