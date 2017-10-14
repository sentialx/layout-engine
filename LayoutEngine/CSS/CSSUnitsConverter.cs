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

            return pxPerCM * val;
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

        /// <summary>
        //  Calculates pixels from points
        /// </summary>
        public static float ptToPX (float val)
        {
            return val * (Utils.getDPI() / 72);
        }

        #endregion
    }
}
