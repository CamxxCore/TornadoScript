using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GTA;
using GTA.Native;
using GTA.Math;
using ScriptCore;

namespace TornadoScript.Script
{
    public class Vortex : ScriptExtension
    {
        /// <summary>
        /// Scale of the vortex forces.
        /// </summary>
        public float ForceScale { get; } = 4.0f;

        /// <summary>
        /// Maximum distance entites must be from the vortex before we start using forces on them.
        /// </summary>
       // public float MaxEntityDist { get; } = 57.0f;

        /// <summary>
        /// Maximum distance entites must be from the vortex before we start using internal vortext forces on them.
        /// </summary>
        public float InternalForcesDist { get; } = 5.0f;

        /// <summary>
        /// Max particle layers in the vortex.
        /// </summary>
        public int MaxLayers { get; } = 47;

        /// <summary>
        /// The amount of space between particle layers in the vortex.
        /// </summary>
        public float LayerSize { get; } = 22f;

        /// <summary>
        /// Amount of <see cref="Particle"/> objects to be placed around the circumference of a layer in the vortex.
        /// </summary>
        public int ParticleCount { get; } = 9;

        /// <summary>
        /// Max height.
        /// </summary>
        public float Height => MaxLayers * LayerSize;

        /// <summary>
        /// Eliptical radius width.
        /// </summary>
        public float RadiusA { get; set; } = 9.40f;

        /// <summary>
        /// Eliptical radius length.
        /// </summary>
        public float RadiusB { get; set; } = 9.40f;

        /// <summary>
        /// Particle rotation speed.
        /// </summary>
        public float Speed { get; set; } = 2.4f;

        readonly List<Particle> _particles = new List<Particle>();

        readonly List<GameSound> _loadedSounds = new List<GameSound>();

        private int _nextUpdateTime;

        private struct ActiveEntity
        {
            public ActiveEntity(Entity entity, float xBias, float yBias)
            {
                Entity = entity;
                XBias = xBias;
                YBias = yBias;
            }

            public Entity Entity { get; }
            public float XBias { get;}
            public float YBias { get; }
        }

        public const int MaxEntityCount = 600;

        private readonly ActiveEntity[] _activeEntities = 
            new ActiveEntity[MaxEntityCount];

        private int _activeEntityCount;

        Vector3 _position, _destination;

        public Vector3 Position => _position;

        private readonly Ped _player = Game.Player.Character;

        public Vortex(Vector3 initialPosition)
        {
            _position = initialPosition;
            _destination = Util.GetRandomPositionFromCoords(initialPosition, 10.0f);
        }

        public void Build()
        {
            var layerSize = LayerSize;

            var radiusX = RadiusA;

            var radiusY = RadiusB;

            var multiplier = 361 / ParticleCount;

            var particleSize = 3.0f;

            var particleCount = ParticleCount;

            for (var i = 0; i < MaxLayers; i++)
            {
                for (var angle = 0; angle <= particleCount; angle++)
                {
                    // increment the Z axis as we build up.
                    var position = this._position;

                    position.Z += layerSize * i;

                    // place the particles at 360 / 10 on the X axis.
                    var rotation = new Vector3(angle * multiplier, 0, 0);

                    Particle particle;

                    if (i < 2)
                    {
                        particle = new Particle(this, position, rotation, "scr_agencyheistb", "scr_env_agency3b_smoke", radiusX, radiusY, i);

                        particle.StartFx(4.7f);

                        _particles.Add(particle);

                        var sound = new GameSound("Woosh_01", "FBI_HEIST_ELEVATOR_SHAFT_DEBRIS_SOUNDS");

                        sound.Play(particle);

                        _loadedSounds.Add(sound);
                    }

                    particle = new Particle(this, position, rotation, "core", "ent_amb_smoke_foundry", radiusX, radiusY, i);

                    particle.StartFx(particleSize);

                    radiusX += 0.080f * (0.72f * i);

                    radiusY += 0.080f * (0.72f * i);

                    particleSize += 0.01f * (0.12f * i);

                    _particles.Add(particle);
                }

                if (i > MaxLayers - 10)
                {
                    particleCount += 1;
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
            var entities = World.GetAllEntities();

            for (var p = 0; p < entities.Length; p++)
            {
                var entityPos = entities[p].Position;

                if (!(Vector2.Distance(entityPos.Vec2(), _position.Vec2()) < maxDistanceDelta) ||
                    !(entities[p].HeightAboveGround < 300.0f)) continue;

                var entityExistsLocally = false;

                for (var x = 0; x < _activeEntityCount; x++)
                {
                    if (_activeEntities[x].Entity != entities[p]) continue;

                    entityExistsLocally = true;

                    break;
                }

                if (entityExistsLocally) continue;

                if (entities[p] is Ped && entities[p].Handle != _player.Handle && !(entities[p] as Ped).IsRagdoll)
                {
                    Function.Call(Hash.SET_PED_TO_RAGDOLL, entities[p].Handle, 800, 1500, 2, 1, 1, 0);
                }

                PushBackEntity(new ActiveEntity(entities[p], 3.0f * Probability.GetScalar(), 3.0f * Probability.GetScalar()));

                /*   if (entities[p] is Ped && entities[p] != Player)
                {
                    Ped ped = entities[p] as Ped;

                    if (!Function.Call<bool>(Hash.GET_IS_TASK_ACTIVE, ped.Handle, 85))
                    {
                        Function.Call(Hash.TASK_SHOCKING_EVENT_REACT, ped.Handle, shockingEventHandle);

                        ped.AlwaysKeepTask = true;
                    }
                }

                if (entities[p] is Vehicle && Player.CurrentVehicle?.Handle != entities[p].Handle)
                {
                    Vehicle vehicle = entities[p] as Vehicle;

                    if (Probability.GetBoolean(0.01f) && vehicle.IsInAir && vehicle.IsDriveable)
                    {
                        vehicle.Explode();
                    }
                }*/
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdatePulledEntities(float maxDistanceDelta)
        {
            float verticalForce = ScriptThread.GetVar<float>("vortexVerticalPullForce");

            float horizontalForce = ScriptThread.GetVar<float>("vortexHorizontalPullForce");

            float topSpeed = ScriptThread.GetVar<float>("vortexTopEntitySpeed");

            for (var e = 0; e < _activeEntityCount; e++)
            {
                var entity = _activeEntities[e].Entity;

                var dist = Vector2.Distance(entity.Position.Vec2(), _position.Vec2());

                if (dist > (maxDistanceDelta - 12.6f) || entity.HeightAboveGround > 300.0f)
                {
                    RemoveEntity(e);

                    continue;
                }

                var targetPos = new Vector3(_position.X + _activeEntities[e].XBias, _position.Y + _activeEntities[e].YBias, entity.Position.Z);

                var direction = Vector3.Normalize(targetPos - entity.Position);

                var forceBias = Probability.NextFloat();

                var force = ForceScale * (forceBias + (forceBias / dist));

             /*   if (dist < InternalForcesDist) // if an entity is close enough to the center, eject them randomly out of the tornado...
                {
                    direction = -direction;

                    entity.ApplyForceToCenterOfMass(direction * force * 10.0f);

                    var cross = Vector3.Cross(direction * force * 10.0f, Vector3.Cross(direction, entity.ForwardVector));

                    entity.ApplyForceToCenterOfMass(Vector3.Normalize(cross) * force);

                    RemoveEntity(e);

                    continue;
                }

                else
                {*/

                    if (entity.Handle == _player.Handle) // we won't update the player if there is something between them and the tornado...
                    {
                        var raycast = World.Raycast(entity.Position, targetPos, IntersectOptions.Map);

                        if (raycast.DitHitAnything)
                        {
                            continue;
                        }

                        verticalForce *= 0.45f;

                        horizontalForce *= 0.5f;
                    }

                if (entity.Model.IsPlane)
                {
                    force *= 6.0f;
                    verticalForce *= 6.0f;
                }

                    entity.ApplyForce(direction * horizontalForce, new Vector3(Probability.NextFloat(), 0, Probability.GetScalar())); // apply a directional force pulling them into the tornado...

                    var upDir = Vector3.Normalize(new Vector3(_position.X, _position.Y, _position.Z + 1000.0f) - entity.Position);

                    entity.ApplyForceToCenterOfMass(upDir * verticalForce); // apply vertical forces

                    var cross = Vector3.Cross(direction, Vector3.WorldUp); //get a vector that is oriented horizontally relative to the vortex.

                    entity.ApplyForceToCenterOfMass(Vector3.Normalize(cross) * force * horizontalForce); // move them along side the vortex.

            //    }

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
                    _destination = Util.GetRandomPositionFromCoords(_position, 10.0f);

                    _destination.Z = World.GetGroundHeight(_destination) - 10.0f;
                }

                _position = MathEx.MoveTowards(_position, _destination, ScriptThread.GetVar<float>("vortexMoveSpeedScale") * 0.287f);
            }

            float maxEntityDist = ScriptThread.GetVar<float>("vortexMaxEntityDist");

            if (gameTime > _nextUpdateTime)
            {
                CollectNearbyEntities(maxEntityDist);

                _nextUpdateTime = gameTime + 800;
            }

            UpdatePulledEntities(maxEntityDist);
        }
      
        public override void Dispose()
        {
            _loadedSounds.ForEach(x => x.Destroy());

            _particles.ForEach(x => x.Dispose());

            base.Dispose();
        }
    }
}
