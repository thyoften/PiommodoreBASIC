using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiommodoreBASIC
{
    public static class Scratchpad
    {
        const int SCR_SIZE = 32768;

        private static double[] scratchpadData = new double[SCR_SIZE];

        public static int Size => scratchpadData.Length;
        public static void InsertAt(int index, double value)
        {
            if (index >= scratchpadData.Length || index < 0)
                throw new Exception("Out of bounds!");

            scratchpadData[index] = value;
        }

        public static double Read(int index)
        {
            if (index >= scratchpadData.Length || index < 0)
                throw new Exception("Out of bounds!");

            return scratchpadData[index];
        }

    }
}
