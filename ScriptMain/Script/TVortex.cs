<<<<<<< HEAD
﻿using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using TornadoScript.ScriptCore.Game;
using TornadoScript.ScriptMain.Memory;
=======
﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GTA;
using GTA.Math;
using GTA.Native;
using TornadoScript.ScriptCore.Game;
>>>>>>> 46660d5b9e2a5942c1c3eb32c40357e5d9abfc48
using TornadoScript.ScriptMain.Utility;

namespace TornadoScript.ScriptMain.Script
{
    public class TornadoVortex : ScriptExtension
    {
        /// <summary>
        /// Scale of the vortex forces.
        /// </summary>
<<<<<<< HEAD
        public float ForceScale { get; } = 3.0f;
=======
        public float ForceScale { get; } = 4.0f;
>>>>>>> 46660d5b9e2a5942c1c3eb32c40357e5d9abfc48

        /// <summary>
        /// Maximum distance entites must be from the vortex before we start using internal vortext forces on them.
        /// </summary>
        public float InternalForcesDist { get; } = 5.0f;

        readonly List<TornadoParticle> _particles = new List<TornadoParticle>();

<<<<<<< HEAD
        private int _aliveTime, _createdTime, _nextUpdateTime;

        private int _lastDebrisSpawnTime = 0;

        private int _lastFullUpdateTime;

        private int _lifeSpan;
=======
        readonly List<GameSound> _loadedSounds = new List<GameSound>();

        private int _nextUpdateTime;
>>>>>>> 46660d5b9e2a5942c1c3eb32c40357e5d9abfc48

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

<<<<<<< HEAD
        public const int MaxEntityCount = 300;

        private readonly Dictionary<int, ActiveEntity> _pulledEntities = new Dictionary<int, ActiveEntity>();

        private readonly List<int> pendingRemovalEntities = new List<int>();

        private Vector3 _position, _destination;

        private bool _despawnRequested;

        public Vector3 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public bool DespawnRequested
        {
            get { return _despawnRequested; }
            set { _despawnRequested = value; }
        }
=======
        public const int MaxEntityCount = 600;

        private readonly ActiveEntity[] _activeEntities =
            new ActiveEntity[MaxEntityCount];

        private int _activeEntityCount;

        private Vector3 _position, _destination;

        public Vector3 Position => _position;
>>>>>>> 46660d5b9e2a5942c1c3eb32c40357e5d9abfc48

        private readonly Ped _player = Helpers.GetLocalPed();

        private int _lastPlayerShapeTestTime;

        bool _lastRaycastResultFailed;

<<<<<<< HEAD
        private materials lastMaterialTraversed;

        private int lastParticleShapeTestTime = 0;

        private Color particleColorPrev, particleColorGoal;

        private Color particleColor = Color.Black;

        private float particleLerpTime = 0.0f;

        private const float ColorLerpDuration = 200.0f;

        private bool _useInternalEntityArray = false;

        // todo: Add crosswinds at vortex base w/ raycast
        public TornadoVortex(Vector3 initialPosition, bool neverDespawn)
        {
            _position = initialPosition;
             _createdTime = Game.GameTime;
            _lifeSpan = neverDespawn ? -1 : Probability.GetInteger(160000, 600000);
            _useInternalEntityArray = ScriptThread.GetVar<bool>("vortexUseEntityPool");
        }

        public void ChangeDestination(bool trackToPlayer )
        {
            for (int i = 0; i < 50; i++)
            {
                _destination = trackToPlayer ? _player.Position.Around(130.0f) : Helpers.GetRandomPositionFromCoords(_destination, 100.0f);

                _destination.Z = World.GetGroundHeight(_destination) - 10.0f;

                var nearestRoadPos = World.GetNextPositionOnStreet(_destination);

                if (_destination.DistanceTo(nearestRoadPos) < 40.0f && Math.Abs(nearestRoadPos.Z - _destination.Z) < 10.0f)
                {
                    break;
                }
            }
=======
        public TornadoVortex(Vector3 initialPosition)
        {
            _position = initialPosition;
            _destination = Helpers.GetRandomPositionFromCoords(initialPosition, 10.0f);
>>>>>>> 46660d5b9e2a5942c1c3eb32c40357e5d9abfc48
        }

        public void Build()
        {
<<<<<<< HEAD
            float radius = ScriptThread.GetVar<float>("vortexRadius");

            int particleCount = ScriptThread.GetVar<int>("vortexParticleCount");

            int maxLayers = ScriptThread.GetVar<int>("vortexMaxParticleLayers");

            string particleAsset = ScriptThread.GetVar<string>("vortexParticleAsset");

            string particleName = ScriptThread.GetVar<string>("vortexParticleName");

            bool enableClouds = ScriptThread.GetVar<bool>("vortexEnableCloudTopParticle");

            bool enableDebris = ScriptThread.GetVar<bool>("vortexEnableCloudTopParticleDebris");

            var multiplier = 360 / particleCount;

            var particleSize = 3.0685f;

            maxLayers = enableClouds ? 12 : maxLayers; // cannot spawn top particles with more than 12 layers!!

            for (var layerIdx = 0; layerIdx < maxLayers; layerIdx++)
            {
                //var lyrParticleNum = (i > maxLayers - 4 ? particleCount + 5 : particleCount);

                //multiplier = 360 / lyrParticleNum;
                for (var angle = 0; angle < (layerIdx > maxLayers - 4 ? particleCount + 5 : particleCount); angle++)
=======
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
>>>>>>> 46660d5b9e2a5942c1c3eb32c40357e5d9abfc48
                {
                    // increment the Z axis as we build up.
                    var position = _position;

<<<<<<< HEAD
                    position.Z += ScriptThread.GetVar<float>("vortexLayerSeperationScale") * layerIdx;
=======
                    position.Z += layerSize * i;
>>>>>>> 46660d5b9e2a5942c1c3eb32c40357e5d9abfc48

                    // place the particles at 360 / 10 on the X axis.
                    var rotation = new Vector3(angle * multiplier, 0, 0);

                    TornadoParticle particle;

<<<<<<< HEAD
                    bool bIsTopParticle = false;

                    if (layerIdx < 2) //debris layer
                    {
                        particle = new TornadoParticle(this, position, rotation, "scr_agencyheistb", "scr_env_agency3b_smoke", radius, layerIdx);
=======
                    if (i < 2 || i > maxLayers - 3)
                    {
                        particle = new TornadoParticle(this, position, rotation, "scr_agencyheistb",
                            "scr_env_agency3b_smoke", radius, i);
>>>>>>> 46660d5b9e2a5942c1c3eb32c40357e5d9abfc48

                        particle.StartFx(4.7f);

                        _particles.Add(particle);
<<<<<<< HEAD

                        Function.Call(Hash.ADD_SHOCKING_EVENT_FOR_ENTITY, 86, particle.Ref.Handle, 0.0f); // shocking event at outer vorticies
                    }

                    if (enableClouds && layerIdx > maxLayers - 3)
                    {
                        if (enableDebris)
                        {
                            particle = new TornadoParticle(this, position, rotation, "scr_agencyheistb", "scr_env_agency3b_smoke", radius * 2.2f, layerIdx);

                            particle.StartFx(12.7f);

                            _particles.Add(particle);
                        }

                        position.Z += 12f;
                        particleSize += 6.0f;

                        radius += 7f;

                        bIsTopParticle = true;
                    }

                    particle = new TornadoParticle(this, position, rotation, particleAsset, particleName, radius, layerIdx, bIsTopParticle);

                    particle.StartFx(particleSize);

                    radius += 0.0799999982118607f * (0.720000028610229f * layerIdx);

                    particleSize += 0.00999999977648258f * (0.119999997317791f * layerIdx);

                    _particles.Add(particle);

                }
            }    
        }

        private void ReleaseEntity(int entityIdx)
        {
            pendingRemovalEntities.Add(entityIdx);
        }

        /// <summary>
        /// Adds a <see cref="ActiveEntity"/> to the queue to be processed next frame
        /// </summary>
        /// <param name="entity"></param>
        private void AddEntity(ActiveEntity entity)
        {
            _pulledEntities[entity.Entity.Handle] = entity;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CollectNearbyEntities(int gameTime, float maxDistanceDelta)
        {
            if (gameTime < _nextUpdateTime)
                return;

            foreach (var ent in MemoryAccess.CollectEntitiesFull())
            {
                if (_pulledEntities.Count >= MaxEntityCount) break;

                if (_pulledEntities.ContainsKey(ent.Handle) ||
                        ent.Position.DistanceTo2D(_position) > maxDistanceDelta + 4.0f || ent.HeightAboveGround > 300.0f) continue;

                if (ent is Ped && /*entities[p].Handle != _player.Handle &&*/ !(ent as Ped).IsRagdoll)
                {
                    Function.Call(Hash.SET_PED_TO_RAGDOLL, ent.Handle, 800, 1500, 2, 1, 1, 0);
                }

                AddEntity(new ActiveEntity(ent, 3.0f * Probability.GetScalar(), 3.0f * Probability.GetScalar()));
            }

            _nextUpdateTime = gameTime + 600;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CollectNearbyEntitiesInternal(int gameTime, float maxDistanceDelta)
        {
            if (gameTime - _lastFullUpdateTime > 5000)
            {
                MemoryAccess.CollectEntitiesFull();

                _lastFullUpdateTime = gameTime;
            }

            if (gameTime > _nextUpdateTime )
            {
                foreach (var ent in MemoryAccess.GetAllEntitiesInternal())
                {
                    if (_pulledEntities.Count >= MaxEntityCount) break;

                    if (_pulledEntities.ContainsKey(ent.Handle) ||
                        ent.Position.DistanceTo2D(_position) > maxDistanceDelta || 
                        ent.HeightAboveGround > 300.0f) continue;

                    if (ent is Ped && !(ent as Ped).IsRagdoll && ent.HeightAboveGround > 2.0f)
                    {
                        Function.Call(Hash.SET_PED_TO_RAGDOLL, ent.Handle, 800, 1500, 2, 1, 1, 0);
                    }

                    AddEntity(new ActiveEntity(ent, 3.0f * Probability.GetScalar(), 3.0f * Probability.GetScalar()));
                }

                _nextUpdateTime = gameTime + 200;
=======
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
>>>>>>> 46660d5b9e2a5942c1c3eb32c40357e5d9abfc48
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdatePulledEntities(int gameTime, float maxDistanceDelta)
        {
            float verticalForce = ScriptThread.GetVar<float>("vortexVerticalPullForce");

            float horizontalForce = ScriptThread.GetVar<float>("vortexHorizontalPullForce");

            float topSpeed = ScriptThread.GetVar<float>("vortexTopEntitySpeed");

<<<<<<< HEAD
            pendingRemovalEntities.Clear();

            foreach (var e in _pulledEntities)
            {
                var entity = e.Value.Entity;

                var dist = Vector2.Distance(entity.Position.Vec2(), _position.Vec2());

                if (dist > maxDistanceDelta - 13f || entity.HeightAboveGround > 300.0f)
                {
                    ReleaseEntity(e.Key);
                    continue;
                }

                var targetPos = new Vector3(_position.X + e.Value.XBias, _position.Y + e.Value.YBias, entity.Position.Z);
=======
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
>>>>>>> 46660d5b9e2a5942c1c3eb32c40357e5d9abfc48

                var direction = Vector3.Normalize(targetPos - entity.Position);

                var forceBias = Probability.NextFloat();

                var force = ForceScale * (forceBias + forceBias / dist);

<<<<<<< HEAD
                if (e.Value.IsPlayer)
                {
                    verticalForce *= 1.62f;

                    horizontalForce *= 1.2f;

                    //  horizontalForce *= 1.5f;

=======
                if (_activeEntities[e].IsPlayer)
                {
>>>>>>> 46660d5b9e2a5942c1c3eb32c40357e5d9abfc48
                    if (gameTime - _lastPlayerShapeTestTime > 1000)
                    {
                        var raycast = World.Raycast(entity.Position, targetPos, IntersectOptions.Map);

                        _lastRaycastResultFailed = raycast.DitHitAnything;

                        _lastPlayerShapeTestTime = gameTime;
                    }

                    if (_lastRaycastResultFailed)
                        continue;
<<<<<<< HEAD
=======

                    verticalForce *= 1.45f;
                    horizontalForce *= 1.5f;
>>>>>>> 46660d5b9e2a5942c1c3eb32c40357e5d9abfc48
                }

                if (entity.Model.IsPlane)
                {
                    force *= 6.0f;
                    verticalForce *= 6.0f;
                }

                // apply a directional force pulling them into the tornado...
                entity.ApplyForce(direction * horizontalForce,
<<<<<<< HEAD
                    new Vector3(Probability.NextFloat(), 0, Probability.GetScalar()));
=======
                    new Vector3(Probability.NextFloat(), 0,
                        Probability.GetScalar()));
>>>>>>> 46660d5b9e2a5942c1c3eb32c40357e5d9abfc48

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
<<<<<<< HEAD

            foreach (var e in pendingRemovalEntities)
            {
                _pulledEntities.Remove(e);
            }
        }

        private static void ApplyDirectionalForce(Entity entity, Vector3 origin, Vector3 direction, float scale)
        {
            if (Function.Call<int>(Hash.GET_VEHICLE_CLASS, entity) == 16 || entity.HeightAboveGround > 15.0f) return;

            float entityDist = Vector3.Distance(entity.Position, origin);

            float zForce, scaleModifier;

            Vector3 rotationalForce;

            if (entity is Vehicle)
            {
                zForce = Probability.GetBoolean(0.50f) ? 0.0332f : 0.0318f;
                scaleModifier = 22.0f;
                rotationalForce = new Vector3(0.0f, 0.1f, 0.40f);
            }

            else if (entity is Ped)
            {
                if (((Ped)entity).IsRagdoll == false)
                    Function.Call(Hash.SET_PED_TO_RAGDOLL, entity.Handle, 800, 1500, 2, 1, 1, 0);
                zForce = 0.0034f;
                scaleModifier = 30.0f;
                rotationalForce = new Vector3(0.0f, 0.0f, 0.12f);
            }

            else
            {
                zForce = 0.000f;
                scaleModifier = 30.0f;
                rotationalForce = new Vector3(0.0f, 0.338f, 0.0f);
            }

            var force = (direction + new Vector3(0, 0, zForce)) * Math.Min(1.0f, scaleModifier / entityDist) * scale;

            entity.ApplyForce(force, rotationalForce, ForceType.MaxForceRot);
        }

        private bool DoEntityCapsuleTest(Vector3 start, Vector3 target, float radius, Entity ignore, out Entity hitEntity)
        {
            var raycastResult = World.RaycastCapsule(start, target, radius, IntersectOptions.Everything, ignore);

            hitEntity = raycastResult.HitEntity;

            return raycastResult.DitHitEntity;
        }

        private void UpdateCrosswinds(int gameTime)
        {
            var forwardLeft = _position + Vector3.WorldNorth * 100.0f;

            var rearLeft = _position - Vector3.WorldNorth * 100.0f;

            var direction = Vector3.Normalize(rearLeft - forwardLeft);

            Entity target;

            if (DoEntityCapsuleTest(forwardLeft, rearLeft, 22.0f, null, out target))
                ApplyDirectionalForce(target, forwardLeft, direction, 4.0f);
        }

        private void UpdateSurfaceDetection(int gameTime)
        {
            if (gameTime - lastParticleShapeTestTime > 1200)
            {
                var str = ShapeTestEx.RunShapeTest(_position + Vector3.WorldUp * 10.0f,
                    _position + Vector3.WorldDown * 10.0f, null, IntersectOptions.Everything);

                if (str.HitMaterial != lastMaterialTraversed)
                {
                    switch (lastMaterialTraversed)
                    {
                        case materials.sand_track:
                        case materials.sand_compact:
                        case materials.sand_dry_deep:
                        case materials.sand_loose:
                        case materials.sand_wet:
                        case materials.sand_wet_deep:
                        {
                            particleColorPrev = particleColor;
                            particleColorGoal = Color.NavajoWhite;
                            particleLerpTime = 0.0f;
                        }

                            break;
                        default:
                            particleColorPrev = particleColor;
                            particleColorGoal = Color.Black;
                            particleLerpTime = 0.0f;
                            break;
                    }

                    lastMaterialTraversed = str.HitMaterial;
                }

                lastParticleShapeTestTime = gameTime;
            }

            if (particleLerpTime < 1.0f)
            {
                particleLerpTime += Game.LastFrameTime / ColorLerpDuration;
                particleColor = particleColor.Lerp(particleColorGoal, particleLerpTime);
            }

            MemoryAccess.SetPtfxColor("core", "ent_amb_smoke_foundry", 0, particleColor);
            MemoryAccess.SetPtfxColor("core", "ent_amb_smoke_foundry", 1, particleColor);
            MemoryAccess.SetPtfxColor("core", "ent_amb_smoke_foundry", 2, particleColor);
        }

        private void UpdateDebrisLayer(materials material)
        {
            if (Game.GameTime - _lastDebrisSpawnTime > 3000 + Probability.GetInteger(0, 5000))
            {
              //  UI.ShowSubtitle("spawn debris");
                
                new TDebris(this, _position, ScriptThread.GetVar<float>("vortexRadius"));
            }
=======
>>>>>>> 46660d5b9e2a5942c1c3eb32c40357e5d9abfc48
        }

        public override void OnUpdate(int gameTime)
        {
<<<<<<< HEAD
            if (gameTime - _createdTime > _lifeSpan)
                _despawnRequested = true;

            if (ScriptThread.GetVar<bool>("vortexEnableSurfaceDetection"))
                UpdateSurfaceDetection(gameTime);

            if (ScriptThread.GetVar<bool>("vortexMovementEnabled"))
            {
                if (_destination == Vector3.Zero || _position.DistanceTo(_destination) < 15.0f)
                {
                    ChangeDestination(false);
                }

                 if (_position.DistanceTo(_player.Position) > 200.0f)
                 {
                     ChangeDestination(true);
                 }

                var vTarget = MathEx.MoveTowards(_position, _destination, ScriptThread.GetVar<float>("vortexMoveSpeedScale") * 0.287f);

                _position = Vector3.Lerp(_position, vTarget, Game.LastFrameTime * 20.0f);
=======
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
>>>>>>> 46660d5b9e2a5942c1c3eb32c40357e5d9abfc48
            }

            float maxEntityDist = ScriptThread.GetVar<float>("vortexMaxEntityDist");

<<<<<<< HEAD
            CollectNearbyEntities(gameTime, maxEntityDist);

            UpdatePulledEntities(gameTime, maxEntityDist);

            UpdateDebrisLayer(lastMaterialTraversed);
            // UpdateCrosswinds(gameTime);
=======
            if (gameTime > _nextUpdateTime)
            {
                CollectNearbyEntities(maxEntityDist);

                _nextUpdateTime = gameTime + 800;
            }

            UpdatePulledEntities(gameTime, maxEntityDist);
>>>>>>> 46660d5b9e2a5942c1c3eb32c40357e5d9abfc48
        }

        public override void Dispose()
        {
<<<<<<< HEAD
=======
            _loadedSounds.ForEach(x => x.Destroy());

>>>>>>> 46660d5b9e2a5942c1c3eb32c40357e5d9abfc48
            _particles.ForEach(x => x.Dispose());

            base.Dispose();
        }
    }
}
