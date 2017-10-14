using System;
using System.Drawing;

namespace LayoutEngine
{
    public class CSSUnitsConverter
    {

        #region Absolute Lengths

        /// <summary>
        //  Calculates pixels from centimeters
        /// </summary>
        public static float CmToPX(float val)
        {
            int dpi = Utils.GetDPI();
            float pxPerCM = dpi / 2.54f; // Get pixels per centimeter

            return pxPerCM * val;
        }

        /// <summary>
        //  Calculates pixels from millimeters
        /// </summary>
        public static float MmToPX (float val)
        {
            return CmToPX(val / 10f);
        }

        /// <summary>
        //  Calculates pixels from inches
        /// </summary>
        public static float InToPX (float val)
        {
            return val * Utils.GetDPI();
        }

        /// <summary>
        //  Calculates pixels from points
        /// </summary>
        public static float PtToPX (float val)
        {
            return Utils.GetDPI() / 72f * val;
        }

        /// <summary>
        //  Calculates pixels from picas
        /// </summary>
        public static float PcToPX(float val)
        {
            return PtToPX(12f * val);
        }

        #endregion
    }
}
