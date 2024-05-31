using System;
using System.Collections.Generic;
using System.Linq;

namespace GaleForce.GPT.Utilities
{
    public static class EmbedHelpers
    {
        public static double[] ToDoubleArray(this byte[] data)
        {
            double[] ddata = new double[data.Length / sizeof(double)];
            Buffer.BlockCopy(data, 0, ddata, 0, data.Length);
            return ddata;
        }

        public static byte[] ToByteArray(this double[] data)
        {
            byte[] bdata = new byte[data.Length * sizeof(double)];
            Buffer.BlockCopy(data, 0, bdata, 0, bdata.Length);
            return bdata;
        }

        public static float[] ToFloatArray(this byte[] byteArray)
        {
            int floatCount = byteArray.Length / sizeof(float);
            float[] floatArray = new float[floatCount];
            Buffer.BlockCopy(byteArray, 0, floatArray, 0, byteArray.Length);
            return floatArray;
        }

        public static byte[] ToByteArray(this float[] floatArray)
        {
            int byteCount = floatArray.Length * sizeof(float);
            byte[] byteArray = new byte[byteCount];
            Buffer.BlockCopy(floatArray, 0, byteArray, 0, byteCount);
            return byteArray;
        }

        public static float[] ToFloatArray(this double[] data)
        {
            return data.Select(x => (float)x).ToArray();
        }

        public static double EuclideanDistance(this float[] vec1, float[] vec2)
        {
            double distance = 0;
            for (int i = 0; i < vec1.Length; i++)
            {
                distance += Math.Pow(vec1[i] - vec2[i], 2);
            }

            return Math.Sqrt(distance);
        }

        public static double EuclideanDistance(this byte[] bvec1, byte[] bvec2)
        {
            var vec1 = bvec1.ToFloatArray();
            var vec2 = bvec2.ToFloatArray();
            double distance = 0;
            for (int i = 0; i < vec1.Length; i++)
            {
                distance += Math.Pow(vec1[i] - vec2[i], 2);
            }

            return Math.Sqrt(distance);
        }

        public static Dictionary<int, double> GetEuclideanDistance(
            Dictionary<int, byte[]> data,
            byte[] target)
        {
            var targetVec = target.ToFloatArray();
            var distances = new Dictionary<int, double>();
            foreach (var item in data)
            {
                var value = item.Value.ToFloatArray();
                var distance = value.EuclideanDistance(targetVec);
                distances.Add(item.Key, distance);
            }

            return distances;
        }

        public static Dictionary<string, double> GetEuclideanDistance(
            Dictionary<string, byte[]> data,
            byte[] target)
        {
            var targetVec = target.ToFloatArray();
            var distances = new Dictionary<string, double>();
            foreach (var item in data)
            {
                var value = item.Value.ToFloatArray();
                var distance = value.EuclideanDistance(targetVec);
                distances.Add(item.Key, distance);
            }

            return distances;
        }
    }
}
