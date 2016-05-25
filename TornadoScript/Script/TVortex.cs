using System;
using System.Linq;
using System.Collections.Generic;
using GTA.Native;
using GTA.Math;

namespace TornadoScript.Script
{
    public class TVortex : IDisposable
    {
        /// <summary>
        /// Scale of the vortex forces.
        /// </summary>
        private const float ForceScale = 1.0f;

        /// <summary>
        /// Maximum distance entites must be from the vortex before we start using forces on them.
        /// </summary>
        private const float MaxEntityDist = 47.0f;

        /// <summary>
        /// Max particle layers in the vortex.
        /// </summary>
        private const int MaxLayers = 30;

        /// <summary>
        /// The amount of space between particle layers in the vortex.
        /// </summary>
        private const float LayerSize = 21f;

        /// <summary>
        /// Amount of <see cref="TParticle"/> objects to be placed around the circumference of a layer in the vortex.
        /// </summary>
        private const int ParticleCount = 10;

        List<TParticle> particles = new List<TParticle>();

        private int nextUpdateTime;

        private List<GTA.Entity> activeEntities =
            new List<GTA.Entity>();

        Vector3 position, destination;

        public Vector3 Position { get { return position; } }

        public TVortex(Vector3 initialPosition)
        {
            position = initialPosition;
            destination = Util.GetRandomPositionFromCoords(initialPosition, 10.0f);
        }

        public void Build()
        {
            int multiplier = 360 / ParticleCount;

            for (int i = 0; i < MaxLayers; i++)
            {
                for (int angle = 0; angle < ParticleCount; angle++)
                {
                    // increment the Z axis as we build up.
                    var position = this.position;
                    position.Z += LayerSize * i;

                    // place the particles at 360 / 10 on the X axis.
                    var rotation = new Vector3(angle * multiplier, 0, 0);

                    TParticle particle;

                    if (i < 2)
                    {
                        particle = new TParticle(position, rotation);
                        particle.SetPTFX("scr_agencyheistb", "scr_env_agency3b_smoke");
                        particle.SetScale(4f);

                        particles.Add(particle);          
                    }

                    particle = new TParticle(position, rotation);

                    particle.SetScale(3.0f);

                   // particle.RadiusA += i * 1.2f;
                   // particle.RadiusB += i * 1.2f;

                    particles.Add(particle);
                }
            }
        }

        public void Update()
        {
            if ((position - GTA.Game.Player.Character.Position).Length() > 200.0f)
            {
                destination = GTA.Game.Player.Character.Position.Around(50f);
            }

            else if ((position - destination).Length() < 15.0f)
            {
                destination = Util.GetRandomPositionFromCoords(position, 10.0f);
            }

            position = Util.MoveTowards(position, destination, 0.287f);
            position.Z = GTA.World.GetGroundHeight(position);

            particles.ForEach(x => x.Update(position));

            if (GTA.Game.GameTime > nextUpdateTime)
            {
                for (int i = 0; i < MaxLayers; i++)
                {
                    var position = this.position;

                    position.Z += LayerSize * i;

                    var nearbyEntities = GTA.World.GetNearbyEntities(position, MaxEntityDist);

                    for (int p = 0; p < nearbyEntities.Length; p++)
                    {
                        if (!activeEntities.Any(x => x.Handle == nearbyEntities[p].Handle))
                            activeEntities.Add(nearbyEntities[p]);
                    }
                }

                nextUpdateTime = GTA.Game.GameTime + 3000;
            }

            for (int e = activeEntities.Count - 1; e > -1; e--)
            {
                var entity = activeEntities[e];

                if ((entity.Position - position).Length() > MaxEntityDist)
                {
                    activeEntities.RemoveAt(e);
                }

                else
                {
                    var dir = Vector3.Normalize(position - entity.Position);

                    if (entity.Handle == GTA.Game.Player.Character.Handle)
                    {
                        var raycast = GTA.World.Raycast(GTA.Game.Player.Character.Position, position, GTA.IntersectOptions.Map);

                        if (!raycast.DitHitAnything)
                        {
                            entity.ApplyForce(dir * 2f);

                            var cross = Vector3.Cross(dir * 2f, Vector3.Cross(dir, entity.ForwardVector));

                            entity.ApplyForce(Vector3.Normalize(cross));

                            entity.ApplyForce(entity.UpVector * 2f);
                        }
                    }

                    else
                    {
                        entity.ApplyForce(dir * ForceScale);

                        var cross = Vector3.Cross(dir, Vector3.Cross(dir, entity.ForwardVector));

                        entity.ApplyForce(Vector3.Normalize(cross) * ForceScale);

                        entity.ApplyForce(entity.UpVector * 1.6f);

                        if (entity is GTA.Ped && !(entity as GTA.Ped).IsRagdoll)
                        {
                            Function.Call(Hash.SET_PED_TO_RAGDOLL, entity.Handle, 800, 1500, 2, 1, 1, 0);
                        }

                        if (entity is GTA.Vehicle && new Random().Next(0, 10000) < 5 && !Function.Call<bool>(Hash.IS_VEHICLE_ALARM_ACTIVATED, entity.Handle))
                        {
                            Function.Call(Hash.START_VEHICLE_ALARM, entity.Handle);
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            particles.ForEach(x => { x.RemoveFX(); x.Delete(); });
        }
    }
}
