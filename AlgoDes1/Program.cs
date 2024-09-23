using System;
using System.IO;
using System.Text;

namespace AlgoDes1
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            ModifiedBalancedSort.BalancedMultiwayMergeSort("entryFile10MB.txt");
        }

        private static void Generate()
        {
            Random random = new Random();
            using (FileStream file = new FileStream("entryFile1GBRandom2.txt", FileMode.Create))
            {
                for (int j = 1; j <= 140_000_000; j++)
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(random.Next(1, 10000000) + " ");
                    file.Write(buffer, 0, buffer.Length);
                }
            }
        }
    }
}