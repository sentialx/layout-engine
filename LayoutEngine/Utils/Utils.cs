using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace LayoutEngine
{
    class Utils
    {
        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hWnd);

        public static List<int> GetIndexes(string parent, string str)
        {
            List<int> indexes = new List<int>();
            int i = -1;

            while ((i = parent.IndexOf(str, i + 1)) != -1)
            {
                indexes.Add(i);
            }

            return indexes;
        }

        public static int GetDPI ()
        {
            return GetDeviceCaps(GetDC(IntPtr.Zero), 88); // 96
        }

        public static float CalculatePercent (float firstNumber, float firstProcent, float secondNumber, float secondProcent)
        {
            if (firstNumber == 0) return secondNumber * firstProcent / secondProcent;
            else if (firstProcent == 0) return firstNumber * secondProcent / secondNumber;
            else if (secondNumber == 0) return firstNumber * secondProcent / firstProcent;
            else if (secondProcent == 0) return secondNumber * firstProcent / firstNumber;

            return 0;
        }
    }
}