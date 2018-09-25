using System;
using System.Security.Cryptography;

namespace TornadoScript.ScriptMain.Utility
{
    /// <summary>
    /// http://stackoverflow.com/questions/1399039/best-way-to-seed-random-in-singleton
    /// </summary>
    public static class StrongRandom
    {
        [ThreadStatic]
        private static Random _random;

        public static int Next(int inclusiveLowerBound, int inclusiveUpperBound)
        {
            if (_random == null)
            {
                var cryptoResult = new byte[4];
                new RNGCryptoServiceProvider().GetBytes(cryptoResult);

                int seed = BitConverter.ToInt32(cryptoResult, 0);

                _random = new Random(seed);
            }

            // upper bound of Random.Next is exclusive
            int exclusiveUpperBound = inclusiveUpperBound + 1;
            return _random.Next(inclusiveLowerBound, exclusiveUpperBound);
        }
    }
}
