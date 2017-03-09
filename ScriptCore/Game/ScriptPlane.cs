using System;
using GTA;
using GTA.Native;

namespace ScriptCore
{
    /// <summary>
    /// Represents a plane.
    /// </summary>
    public class ScriptPlane : ScriptEntity<Vehicle>
    {
        /// <summary>
        /// Fired when the vehicle is no longer drivable.
        /// </summary>
        public event ScriptEntityEventHandler Undrivable;

        /// <summary>
        /// State of the vehicle landing gear.
        /// </summary>
        public LandingGearState LandingGearState
        {
            get { return (LandingGearState)Function.Call<int>(Hash._GET_VEHICLE_LANDING_GEAR, Ref.Handle); }
            set { Function.Call(Hash._SET_VEHICLE_LANDING_GEAR, Ref.Handle, (int)value); }
        }

        private int undrivableTicks = 0;

        public ScriptPlane(Vehicle baseRef) : base(baseRef)
        { }

        protected virtual void OnUndrivable(ScriptEntityEventArgs e)
        {
            Undrivable?.Invoke(this, e);
        }

        public override void OnUpdate(int gameTime)
        {
            if (!Ref.IsDriveable)
            {
                if (undrivableTicks == 0)
                    OnUndrivable(new ScriptEntityEventArgs(gameTime));

                undrivableTicks++;
            }

            else
            {
                undrivableTicks = 0;
            }

            base.OnUpdate(gameTime);
        }
    }

    public enum LandingGearState
    {
        Deployed,
        Closing,
        Opening,
        Retracted
    }
}
