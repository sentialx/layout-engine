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
            return cmToPX(val / 10f);
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
            return Utils.getDPI() / 72f * val;
        }

        /// <summary>
        //  Calculates pixels from picas
        /// </summary>
        public static float pcToPX(float val)
        {
            return ptToPX(12f * val);
        }

        #endregion
    }
}
