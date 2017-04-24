using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Vec2(this Vector3 v)
        {
            return new Vector2(v.X, v.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ApplyForceToCenterOfMass(this Entity entity, Vector3 force)
        {
            Function.Call(Hash.APPLY_FORCE_TO_ENTITY_CENTER_OF_MASS, entity.Handle, 1, force.X, force.Y, force.Z, 0, 0, 1, 1);
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

        public static float Lerp(this float a, float b, float f)
        {
            return (a * (1.0f - f)) + (b * f);
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
