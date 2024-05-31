namespace TestGaleForce.GPT
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using GaleForce.GPT.Utilities;

    [TestClass]
    public class EmbedUtilTest
    {
        [TestMethod]
        public void TestConvert()
        {
            var data = this.ByteData();
            Assert.IsTrue(AreByteArraysIdentical(data, data.ToFloatArray().ToByteArray()));
            Assert.IsTrue(AreByteArraysIdentical(data, data.ToDoubleArray().ToByteArray()));
        }

        public byte[] ByteData()
        {
            byte[] byteArray = new byte[64];

            // Seed for random number generation
            int seed = 12345;
            Random random = new Random(seed);

            // Fill the array with random data
            random.NextBytes(byteArray);
            return byteArray;
        }

        private static bool AreByteArraysIdentical(byte[] array1, byte[] array2)
        {
            if (array1 == null || array2 == null)
            {
                return false;
            }

            if (array1.Length != array2.Length)
            {
                return false;
            }

            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
