using System;
using GTA;

namespace TornadoScript.ScriptCore.Game
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
        public int TotalTicks { get; private set; }

        /// <summary>
        /// Total time entity has been available to the script.
        /// </summary>
        public TimeSpan TotalTime { get; private set; }

        /// <summary>
        /// Time at which the entity was made avilable to the script.
        /// </summary>
        public int CreatedTime { get; }

        /// <summary>
        /// Initialize the class.
        /// </summary>
        /// <param name="baseRef"></param>
        protected ScriptEntity(T baseRef)
        {
            Ref = baseRef;
            CreatedTime = GTA.Game.GameTime;
        }

        /// <summary>
        /// Call this method each tick to update entity related information.
        /// </summary>
        public override void OnUpdate(int gameTime)
        {        
            TotalTicks++;

            TotalTicks = TotalTicks % int.MaxValue;

            TotalTime = TimeSpan.FromMilliseconds(gameTime - CreatedTime);
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

        public static implicit operator Entity(ScriptEntity<T> e)
        {
            return e.Ref;
        }
    }
}
