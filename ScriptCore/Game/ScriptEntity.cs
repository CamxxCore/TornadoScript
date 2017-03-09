using System;
using GTA;

namespace ScriptCore
{
    /// <summary>
    /// Represents a game entity.
    /// </summary>
    public abstract class ScriptEntity<T> : ScriptExtension, IScriptEntity where T : Entity
    {
        /// <summary>
        /// Base game entity reference.
        /// </summary>
        public T Ref { get; }

        /// <summary>
        /// Total entity ticks.
        /// </summary>
        public int TotalTicks
        {
            get { return totalTicks; }
        }

        /// <summary>
        /// Total time entity has been available to the script.
        /// </summary>
        public TimeSpan TotalTime
        {
            get { return totalTime; }
        }

        /// <summary>
        /// Time at which the entity was made avilable to the script.
        /// </summary>
        public int CreatedTime
        {
            get { return createdTime; }
        }

        private TimeSpan totalTime;

        private int createdTime;

        private int deadTicks, aliveTicks, waterTicks, totalTicks;

        /// <summary>
        /// Initialize the class.
        /// </summary>
        /// <param name="baseRef"></param>
        public ScriptEntity(T baseRef)
        {
            Ref = baseRef;
            createdTime = Game.GameTime;
        }

        /// <summary>
        /// Call this method each tick to update entity related information.
        /// </summary>
        public override void OnUpdate(int gameTime)
        {        
            totalTicks++;

            totalTicks = totalTicks % int.MaxValue;

            totalTime = TimeSpan.FromMilliseconds(gameTime - createdTime);
        }

        public void Remove()
        {
            Ref.CurrentBlip?.Remove();

            Ref.Delete();
        }

        public override void Dispose()
        {
            Remove();

            base.Dispose();
        }

        public static implicit operator Entity(ScriptEntity<T> e)  // implicit conversion ScriptEntity <-> Entity
        {
            return e.Ref;
        }
    }
}
