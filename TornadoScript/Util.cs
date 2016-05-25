using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA;
using GTA.Native;
using GTA.Math;
using System.IO;

namespace TornadoScript
{
    public static class Util
    {
        public static Quaternion Euler(float x, float y, float z)
        {
            return Euler(new Vector3(x, y, z));
        }

        public static Vector3 GetRandomPositionFromCoords(Vector3 position, float multiplier)
        {
            float randX, randY;

            int v1 = Function.Call<int>(Hash.GET_RANDOM_INT_IN_RANGE, 0, 3999) / 1000;

            if (v1 == 0)
            {
                randX = Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, 50.0f, 200.0f) * multiplier;
                randY = Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, -50.0f, 50.0f) * multiplier;
            }
            else if (v1 == 1)
            {
                randX = Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, 50.0f, 200.0f) * multiplier;
                randY = Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, -50.0f, 50.0f) * multiplier;
            }
            else if (v1 == 2)
            {
                randX = Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, -50.0f, -200.0f) * multiplier;
                randY = Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, 50.0f, 50.0f) * multiplier;
            }
            else
            {
                randX = Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, 50.0f, -200.0f) * multiplier;
                randY = Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, -50.0f, 50.0f) * multiplier;
            }
            return new Vector3(randX + position.X, randY + position.Y, position.Z);

        }

        public static Vector3 MoveTowards(Vector3 current, Vector3 target, float maxDistanceDelta)
        {
            Vector3 a = target - current;
            float magnitude = a.Length();
            if (magnitude <= maxDistanceDelta || magnitude == 0f)
            {
                return target;
            }

            return current + a / magnitude * maxDistanceDelta;
        }

        public static Quaternion Euler(Vector3 eulerAngles)
        {
            float halfPhi = 0.5f * eulerAngles.X; // Half the roll.
            float halfTheta = 0.5f * eulerAngles.Y; // Half the pitch.
            float halfPsi = 0.5f * eulerAngles.Z; // Half the yaw.

            float cosHalfPhi = (float)Math.Cos(halfPhi);
            float sinHalfPhi = (float)Math.Sin(halfPhi);
            float cosHalfTheta = (float)Math.Cos(halfTheta);
            float sinHalfTheta = (float)Math.Sin(halfTheta);
            float cosHalfPsi = (float)Math.Cos(halfPsi);
            float sinHalfPsi = (float)Math.Sin(halfPsi);

            return new Quaternion(
                cosHalfPhi * cosHalfTheta * cosHalfPsi - sinHalfPhi * sinHalfTheta * sinHalfPsi,
                sinHalfPhi * cosHalfTheta * cosHalfPsi + cosHalfPhi * sinHalfTheta * sinHalfPsi,
                cosHalfPhi * sinHalfTheta * cosHalfPsi - sinHalfPhi * cosHalfTheta * sinHalfPsi,
                cosHalfPhi * cosHalfTheta * sinHalfPsi + sinHalfPhi * sinHalfTheta * cosHalfPsi

            );
        }

        public static Vector3 MultiplyVector(Vector3 vec, Quaternion quat)
        {
            float num = quat.X * 2f;
            float num2 = quat.Y * 2f;
            float num3 = quat.Z * 2f;
            float num4 = quat.X * num;
            float num5 = quat.Y * num2;
            float num6 = quat.Z * num3;
            float num7 = quat.X * num2;
            float num8 = quat.X * num3;
            float num9 = quat.Y * num3;
            float num10 = quat.W * num;
            float num11 = quat.W * num2;
            float num12 = quat.W * num3;
            Vector3 result;
            result.X = (1f - (num5 + num6)) * vec.X + (num7 - num12) * vec.Y + (num8 + num11) * vec.Z;
            result.Y = (num7 + num12) * vec.X + (1f - (num4 + num6)) * vec.Y + (num9 - num10) * vec.Z;
            result.Z = (num8 - num11) * vec.X + (num9 + num10) * vec.Y + (1f - (num4 + num5)) * vec.Z;
            return result;
        }

        public static string[] GetLines(this string s)
        {
            return s.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        }

        /// <summary>
        /// Populates a list of strings from an embedded string resource.
        /// </summary>
        /// <param name="resource">The string resource (Properties.Resources.ProjectName...)</param>
        /// <returns></returns>
        public static IList<string> ReadEmbeddedResource(string resource)
        {
            string[] text = resource.GetLines();
            return new List<string>(text);
        }

        /// <summary>
        /// Writes a list of strings to a file at the specified path.
        /// </summary>
        /// <param name="list">The list to write</param>
        /// <param name="filepath">The specified path</param>
        public static void WriteListToFile(IList<string> list, string filepath)
        {
            if (File.Exists(filepath)) File.Delete(filepath);
            using (StreamWriter stream = new StreamWriter(filepath))
            {
                foreach (string line in list)
                {
                    stream.WriteLine(line);
                }
            }
        }
    }
}
