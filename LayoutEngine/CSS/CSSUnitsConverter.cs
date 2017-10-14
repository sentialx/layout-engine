using System;

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
            float pxPerCM = 37.795276f;

            return pxPerCM * val + (0.1f * val * pxPerCM);
        }

        /// <summary>
        //  Calculates pixels from millimeters
        /// </summary>
        public static float mmToPX (float val)
        {
            return cmToPX(val / 10);
        }

        #endregion
    }
}
