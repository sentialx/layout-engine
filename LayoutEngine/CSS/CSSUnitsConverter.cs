using System;

namespace LayoutEngine
{
    public class CSSUnitsConverter
    {

        /// <summary>
        //  Calculates pixels from centimeters
        /// </summary>
        public static float CmToPX (float val)
        {
            int dpi = Utils.GetDPI();
            float pxPerCM = dpi / 2.54f;

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
        public static float PcToPX (float val)
        {
            return PtToPX(12f * val);
        }

        /// <summary>
        //  Calculates degrees from gradians
        /// </summary>
        public static float GradToDeg (float val)
        {
            return val * 360 / 400;
        }

        /// <summary>
        //  Calculates degrees from radians
        /// </summary>
        public static float RadToDeg(float val)
        {
            return 360f * val / (float)(Math.PI * 2f);
        }

        /// <summary>
        //  Calculates degrees from turns
        /// </summary>
        public static float TurnToDeg(float val)
        {
            return val * 360f;
        }

        /// <summary>
        //  Calculates x times the size of the current font
        /// </summary>
        public static float EmToPx(Rule rule, DOMElement element)
        {
            float fontSize = (element.Parent != null) ? element.Parent.Style.Font.Size : 16; // 16 - Document font-size

            if (element.Style.Font != null)
            {
                fontSize = element.Style.Font.Size;
            }

            return fontSize * rule.ComputedValue.Value;
        }

        /// <summary>
        //  Calculates x percent of viewport height
        /// </summary>
        public static float VhToPx(Rule rule, DOMElement element)
        {
            return rule.ComputedValue.Value / 100 * Program.htmlDocument.getViewport().Height;
        }

        /// <summary>
        //  Calculates x percent of viewport width
        /// </summary>
        public static float VwToPx(Rule rule, DOMElement element)
        {
            return rule.ComputedValue.Value / 100 * Program.htmlDocument.getViewport().Width;
        }

        /// <summary>
        /// Calculates pixels from percent
        /// </summary>
        public static float PercentToPx(Rule rule, DOMElement element)
        {
            if (rule.Property == "width")
            {
                float parentWidth = (element.Parent != null) ? element.Parent.ComputedStyle.Size.Width : Program.deviceWidth;

                return (rule.ComputedValue.ValueBeforeComputing / 100) * parentWidth;
            }
            else if (rule.Property == "height")
            {
                float parentHeight = (element.Parent != null) ? element.Parent.Style.Size.Height : Program.deviceHeight;

                return (rule.ComputedValue.ValueBeforeComputing / 100) * parentHeight;
            }

            return 0f;
        }
    }
}
