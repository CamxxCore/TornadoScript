using System;

namespace TornadoScript.ScriptMain.Utility
{
    public static class Probability
    {
        private static int _lastCheckedTime;

        private static readonly Random Rand = new Random();

        /// <summary>
        /// Gets a random float value
        /// </summary>
        /// <returns></returns>
        public static float GetFloat()
        {
            return GetScalar() * float.MaxValue;
        }


        /// <summary>
        /// Gets a random float value in range
        /// </summary>
        /// <returns></returns>
        public static float GetFloat(float min, float max)
        {
            return NextFloat() * (max - min) + min;
        }

        /// <summary>
        /// Gets a random float value from 0.0 to 1.0
        /// </summary>
        /// <returns></returns>
        public static float NextFloat()
        {
            return (float)Rand.NextDouble();
        }

        /// <summary>
        /// Gets a random float value from -1.0 to 1.0
        /// </summary>
        /// <returns></returns>
        public static float GetScalar()
        {
            var val = Rand.NextDouble();
            val -= 0.5;
            val *= 2;
            return (float) val;
        }

        /// <summary>
        /// Gets a random integer value in range
        /// </summary>
        /// <returns></returns>
        public static int GetInteger(int min, int max)
        {
            return GetInteger(min, max, false);
        }

        /// <summary>
        /// Gets a random integer value
        /// </summary>
        /// <returns></returns>
        public static int GetInteger()
        {
            return GetInteger(0, int.MaxValue, false);
        }

        /// <summary>
        /// Gets a random integer value in range
        /// </summary>
        /// <param name="abs">Return the absolute value.</param>
        /// <returns></returns>
        public static int GetInteger(int min, int max, bool abs)
        {
            var result = StrongRandom.Next(min, max);         
            return abs ? Math.Abs(result) : result;
        }

        /// <summary>
        /// Checks for a conditon given a % of chance and interval
        /// </summary>
        /// <param name="chance">% chance of success</param>
        /// <returns>rand</returns>
        public static bool GetBoolean()
        {
            return GetBoolean(0.5f);
        }

        /// <summary>
        /// Checks for a conditon given a % of chance and interval
        /// </summary>
        /// <param name="chance">% chance of success</param>
        /// <returns>rand</returns>
        public static bool GetBoolean(float chance)
        {
            return GetBoolean(chance, 0);
        }

        public static bool GetBoolean(float chance, int checkInterval)
        {
            if (checkInterval <= 0)
                return StrongRandom.Next(0, 1000) < (int) (chance * 1000.0f);

            if (Environment.TickCount - _lastCheckedTime < checkInterval)
                return false;

            _lastCheckedTime = Environment.TickCount;

            return StrongRandom.Next(0, 1000) < (int)(chance * 1000.0f);
        }
    }
}
