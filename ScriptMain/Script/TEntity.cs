using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptCore;
using GTA.Math;
using GTA.Native;
using GTA;

namespace TornadoScript.Script
{
    public class TEntity : ScriptEntity<Entity>
    {
        private TVortex parent;

        public TEntity(Entity baseRef, TVortex parent) : base(baseRef)
        {
            this.parent = parent;
        }

        public override void OnUpdate(int gameTime)
        {
            var parentPos = parent.Position;

            float dist = Vector2.Distance(Ref.Position.Vec2(), parentPos.Vec2());

            if (dist > parent.MaxEntityDist || Ref.HeightAboveGround > 300.0f)
            {
                Dispose();
            }

            else
            {
                var direction = Vector3.Normalize(new Vector3(parentPos.X, parentPos.Y, Ref.Position.Z) - Ref.Position);

                if (dist < parent.InternalForcesDist)
                {
                    direction = -direction;

                    Ref.ApplyForceToCenterOfMass(direction * 2.0f);

                    var cross = Vector3.Cross(direction * 2.0f, Vector3.Cross(direction, Ref.ForwardVector));

                    Ref.ApplyForceToCenterOfMass(Vector3.Normalize(cross));

                    Dispose();
                }

                else
                {
                    var upDir = Vector3.Normalize(new Vector3(parentPos.X, parentPos.Y, parentPos.Z + 1000.0f) - Ref.Position);

                    Ref.ApplyForce(direction, new Vector3(Probability.NextFloat(),
                        Probability.NextFloat(),
                        Probability.NextFloat()));

                    Ref.ApplyForceToCenterOfMass(direction * parent.ForceScale * (ScriptThread.GetVar<float>("vortexHorzizontalPullForce") / dist));

                    Ref.ApplyForceToCenterOfMass(upDir * ScriptThread.GetVar<float>("vortexVerticalPullForce") * (2.0f / dist));

                    var cross = Vector3.Cross(direction, Vector3.WorldUp);

                    float forceBias = Probability.NextFloat();

                    Ref.ApplyForceToCenterOfMass(Vector3.Normalize(cross) * parent.ForceScale * (((forceBias * 0.6f) + 0.16f) + (forceBias / dist))); // move them horizontally relative to the vortex.

                    if (Probability.GetBoolean(0.23f))
                    {
                        Ref.ApplyForceToCenterOfMass(direction * parent.ForceScale);

                        cross = Vector3.Cross(direction, Vector3.Cross(direction, Ref.ForwardVector));

                        Ref.ApplyForceToCenterOfMass(Vector3.Normalize(cross) * parent.ForceScale);
                    }

                    if (Ref is Ped && Ref.Handle != Game.Player.Character.Handle && !(Ref as Ped).IsRagdoll)
                    {
                        Function.Call(Hash.SET_PED_TO_RAGDOLL, Ref.Handle, 800, 1500, 2, 1, 1, 0);
                    }
                }

                Function.Call(Hash.SET_ENTITY_MAX_SPEED, Ref.Handle, 30.0f);
            }

            base.OnUpdate(gameTime);
        }
    }
}
