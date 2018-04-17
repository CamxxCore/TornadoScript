using System;
using System.Collections.Generic;
using GTA.Math;

namespace TornadoScript.ScriptMain.Utility
{
    public static class MathEx
    {
        private static Dictionary<float, float> _cosTable = new Dictionary<float, float>();

        private static float[] _cos = new float[720];

        private static float[] _sin = new float[720];

        public const double RadToDeg = 180 / Math.PI;

        public const double DegToRad = Math.PI / 180;

        static MathEx()
        {
            for (int i = 0; i < 360; i++)
            {
                _cos[i] = (float) Math.Cos(ToRadians(360 - i));
                _sin[i] = (float) Math.Sin(ToRadians(360 - i));
                _cos[i + 360] = (float)Math.Cos(ToRadians(i));
                _sin[i + 360] = (float)Math.Sin(ToRadians(i));
            }
        }

        public static float Cos(double value)
        {
            int deg = (int)value.ToDegrees();
            return value < 0 ? _cos[-deg] : _cos[deg + 360];
        }

        public static float Sin(double value)
        {
            int deg = (int)value.ToDegrees();
            return value < 0 ? _sin[-deg] : _sin[deg + 360];
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

        public static Vector3 AnglesToForward(Vector3 position, Vector3 angles, int length)
        {
            float num = (float)Math.Sin(angles.X * Math.PI / 180) * length;
            float num1 = (float)Math.Sqrt(length * length - num * num);
            float num2 = (float)Math.Sin(angles.Y * Math.PI / 180) * num1;
            float num3 = (float)Math.Cos(angles.Y * Math.PI / 180) * num1;
            return new Vector3(position.X + num3, position.Y + num2, position.Z - num);
        }

        private static Quaternion AngleAxis(float degress, ref Vector3 axis)
        {
            if (axis.Length() == 0.0f)
                return Quaternion.Identity;

            Quaternion result = Quaternion.Identity;
            var radians = degress * (float)(Math.PI / 180.0);
            radians *= 0.5f;
            axis.Normalize();
            axis = axis * (float)Math.Sin(radians);
            result.X = axis.X;
            result.Y = axis.Y;
            result.Z = axis.Z;
            result.W = (float)Math.Cos(radians);

            result.Normalize();

            return result;
        }

        /// <summary>
        /// Convert degrees to radians.
        /// </summary>
        /// <param name="val">The value in degrees.</param>
        /// <returns></returns>
        public static double ToRadians(this double val)
        {
            return DegToRad * val;
        }


        /// <summary>
        /// Convert degrees to radians.
        /// </summary>
        /// <param name="val">The value in degrees.</param>
        /// <returns></returns>
        public static double ToDegrees(this double val)
        {
            return RadToDeg * val;
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

        public static Quaternion Euler(float x, float y, float z)
        {
            return Euler(new Vector3(x, y, z));
        }
    }
}
