using System;
using System.Drawing;

namespace LayoutEngine
{
    class CSSUnitsConverter
    {
        #region absoluteLengths

        /// <summary>
        //  Calculates pixels from centimeters
        /// </summary>
        public static float cmToPX(float val)
        {
            int dpi = Utils.getDPI();
            float pxPerCM = dpi / 2.54f; // Get pixels per centimeter

            return pxPerCM * val + (0.1f * val * pxPerCM);
        }

        /// <summary>
        //  Calculates pixels from millimeters
        /// </summary>
        public static float mmToPX (float val)
        {
            return cmToPX(val / 10);
        }

        /// <summary>
        //  Calculates pixels from inches
        /// </summary>
        public static float inToPX (float val)
        {
            return val * Utils.getDPI();
        }

        #endregion
    }
}
