using System;
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

        public static float[] ToFloatArray(this byte[] data)
        {
            return data.ToDoubleArray().ToFloatArray();
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
    }
}
