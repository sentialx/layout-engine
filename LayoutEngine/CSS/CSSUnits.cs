using System;

namespace LayoutEngine
{
    class CSSUnits
    {
        #region absoluteLengths

        /// <summary>
        //  Calculates centimeters to pixels
        /// </summary>
        public double cmToPX(int c)
        {
            return 37.795276 * c;
        }

        #endregion
    }
}
