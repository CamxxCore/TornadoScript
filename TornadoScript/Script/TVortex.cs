using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GTA;
using GTA.Native;
using GTA.Math;
using ScriptCore;

namespace TornadoScript.Script
{
    public class TVortex : ScriptExtension, IDisposable
    {
        /// <summary>
        /// Scale of the vortex forces.
        /// </summary>
        public float ForceScale { get; } = 1.0f;

        /// <summary>
        /// Maximum distance entites must be from the vortex before we start using forces on them.
        /// </summary>
        public float MaxEntityDist { get; } = 57.0f;

        /// <summary>
        /// Maximum distance entites must be from the vortex before we start using internal vortext forces on them.
        /// </summary>
        public float InternalForcesDist { get; } = 7.0f;

        /// <summary>
        /// Max particle layers in the vortex.
        /// </summary>
        public int MaxLayers { get; } = 43;

        /// <summary>
        /// The amount of space between particle layers in the vortex.
        /// </summary>
        public float LayerSize { get; } = 21f;

        /// <summary>
        /// Amount of <see cref="TParticle"/> objects to be placed around the circumference of a layer in the vortex.
        /// </summary>
        public int ParticleCount { get; } = 9;

        /// <summary>
        /// Max height.
        /// </summary>
        public float Height { get { return MaxLayers * LayerSize; } }

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

        private int shockingEventHandle = -1;

        List<TParticle> particles = new List<TParticle>();

        List<GameSound> loadedSounds = new List<GameSound>();

        private int nextUpdateTime;

        private List<Entity> activeEntities = new List<Entity>();

        Vector3 position, destination;

        public Vector3 Position { get { return position; } }

        private readonly Ped Player = Game.Player.Character;

        public TVortex(Vector3 initialPosition)
        {
            position = initialPosition;
            destination = Util.GetRandomPositionFromCoords(initialPosition, 10.0f);
        }

        public void Build()
        {
            float layerSize = LayerSize;

            float radiusX = RadiusA;

            float radiusY = RadiusB;

            int multiplier = 360 / ParticleCount;

            for (int i = 0; i < MaxLayers; i++)
            {
                for (int angle = 0; angle <= ParticleCount; angle++)
                {
                    // increment the Z axis as we build up.
                    var position = this.position;

                    position.Z += layerSize * i;

                    // place the particles at 360 / 10 on the X axis.
                    var rotation = new Vector3(angle * multiplier, 0, 0);

                    TParticle particle;

                    if (i < 2)
                    {
                        particle = new TParticle(this, position, rotation, "scr_agencyheistb", "scr_env_agency3b_smoke", RadiusA, RadiusB, i);

                        particle.StartFX(6.0f);

                        particles.Add(particle);
                    }

                    particle = new TParticle(this, position, rotation, "core", "ent_amb_smoke_foundry", radiusX, radiusY, i);

                    particle.StartFX(3.0f);

                    radiusX += 0.069f * (0.72f * i);

                    radiusY += 0.069f * (0.72f * i);

                    if (i < 1)
                    {
                        var sound = new GameSound("Woosh_01", "FBI_HEIST_ELEVATOR_SHAFT_DEBRIS_SOUNDS");

                        sound.Play(particle);

                        loadedSounds.Add(sound);

                        if (shockingEventHandle == -1)
                        {
                            shockingEventHandle = Function.Call<int>(Hash.ADD_SHOCKING_EVENT_FOR_ENTITY, 114, particle.Ref, -1.0f);
                        }
                    }

                    particles.Add(particle);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CollectNearbyEntities()
        {
            var entities = World.GetAllEntities();

            bool entityExistsLocally = false;

            for (int p = 0; p < entities.Length; p++)
            {
                Vector3 entityPos = entities[p].Position;

                if (Vector2.Distance(entityPos.Vec2(), position.Vec2()) < MaxEntityDist && entities[p].HeightAboveGround < 300.0f)
                {
                    entityExistsLocally = false;

                    for (int x = 0; x < activeEntities.Count; x++)
                    {
                        if (activeEntities[x] == entities[p])
                        {
                            entityExistsLocally = true;

                            break;
                        }
                    }

                    if (!entityExistsLocally)
                    {
                        if (entities[p] is Ped &&  entities[p].Handle != Player.Handle && !(entities[p] as Ped).IsRagdoll)
                        {
                            Function.Call(Hash.SET_PED_TO_RAGDOLL, entities[p].Handle, 800, 1500, 2, 1, 1, 0);
                        }

                        activeEntities.Add(entities[p]);
                    }
                }

                if (entities[p] is Ped && entities[p] != Player)
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
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdatePulledEntities()
        {
            float verticalForce = ScriptThread.GetVar<float>("vortexVerticalPullForce");

            float horizontalForce = ScriptThread.GetVar<float>("vortexHorzizontalPullForce");

            Entity entity; float dist;

            for (int e = activeEntities.Count - 1; e > -1; e--)
            {
                entity = activeEntities[e];

                dist = Vector2.Distance(entity.Position.Vec2(), position.Vec2());

                if (dist > (MaxEntityDist - 8.0f) || entity.HeightAboveGround > 300.0f)
                {
                    activeEntities.RemoveAt(e);
                    continue;
                }

                Vector3 targetPos = new Vector3(position.X, position.Y, entity.Position.Z);

                var direction = Vector3.Normalize(targetPos - entity.Position);

                float forceBias = Probability.NextFloat();

                float force = ForceScale * (forceBias + (forceBias / dist));

                if (dist < InternalForcesDist) // if an entity is close enough to the center, eject them randomly out of the tornado...
                {
                    direction = -direction;

                    entity.ApplyForceToCenterOfMass(direction * force * 10.0f);

                    var cross = Vector3.Cross(direction * force * 10.0f, Vector3.Cross(direction, entity.ForwardVector));

                    entity.ApplyForceToCenterOfMass(Vector3.Normalize(cross) * force);

                    activeEntities.RemoveAt(e);
                }

                else
                {
                    var upDir = Vector3.Normalize(new Vector3(position.X, position.Y, position.Z + 1000.0f) - entity.Position);

                    if (entity.Handle == Player.Handle) // we won't update the player if there is something between them and the tornado...
                    {
                        var raycast = World.Raycast(entity.Position, targetPos, IntersectOptions.Map);

                        if (raycast.DitHitAnything)
                        {
                            continue;
                        }

                        verticalForce *= 0.5f;
                    }

                    entity.ApplyForce(direction * horizontalForce, new Vector3(Probability.NextFloat(), Probability.NextFloat(), 0)); // apply a directional force pulling them into the tornado...

                    entity.ApplyForceToCenterOfMass(upDir * force * verticalForce); // apply vertical forces

                    var cross = Vector3.Cross(direction, Vector3.WorldUp); //get a vector that is oriented horizontally relative to the vortex.

                    entity.ApplyForceToCenterOfMass(Vector3.Normalize(cross) * force * horizontalForce); // move them along side the vortex.
                }

                Function.Call(Hash.SET_ENTITY_MAX_SPEED, entity.Handle, 32.0f);
            }
        }

        public override void OnUpdate(int gameTime)
        {
            if (position.DistanceTo(Player.Position) > 200.0f)
            {
                destination = Player.Position.Around(50.0f);

                destination.Z = World.GetGroundHeight(destination);
            }

            else if (position.DistanceTo(destination) < 15.0f)
            {
                destination = Util.GetRandomPositionFromCoords(position, 10.0f);

                destination.Z = World.GetGroundHeight(destination);
            }

            position = MathEx.MoveTowards(position, destination, 0.287f);

            if (gameTime > nextUpdateTime)
            {
                CollectNearbyEntities();

                nextUpdateTime = gameTime + 800;
            }

            UpdatePulledEntities();
        }
      
        public override void Dispose()
        {
            loadedSounds.ForEach(x => x.Destroy());

            particles.ForEach(x => x.Dispose());

            base.Dispose();
        }
    }
}
