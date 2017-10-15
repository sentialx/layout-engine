using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace LayoutEngine
{
    public class CSSUnits
    {
        public static CSSValue ParseValue (Rule rule, DOMElement element)
        {
            CSSValue cssValue = new CSSValue();

            List<string> array = Regex.Split(rule.Value, @"[^0-9\.]+").Where(c => c != "." && c.Trim() != "").ToList();

            if (array.Count > 0 && rule.Value.Length > 2)
            {
                float value = float.Parse(array[0], CultureInfo.InvariantCulture.NumberFormat);
                int startIndex = value.ToString().Length;

                string unitType = rule.Value.Substring(startIndex, rule.Value.Length - startIndex).ToLower();
                cssValue.Value = value;

                if (unitType == "px") cssValue.Unit = Unit.Px;
                else if (unitType == "cm") cssValue.Unit = Unit.Cm;
                else if (unitType == "mm") cssValue.Unit = Unit.Mm;
                else if (unitType == "in") cssValue.Unit = Unit.In;
                else if (unitType == "pt") cssValue.Unit = Unit.Pt;
                else if (unitType == "pc") cssValue.Unit = Unit.Pc;
                else if (unitType == "%") cssValue.Unit = Unit.Percent;
                else if (unitType == "em") cssValue.Unit = Unit.Em;

                rule.ComputedValue = cssValue;

                cssValue.Value = ConvertAnyUnitToPixels(rule, element);
                cssValue.ValueBeforeComputing = value;

                return cssValue;
            }

            return null;
        }

        private static float ConvertAnyUnitToPixels (Rule rule, DOMElement element)
        {
            CSSValue cssValue = rule.ComputedValue;

            if (cssValue.Unit == Unit.Px)
            {
                return cssValue.Value;
            }
            else if (cssValue.Unit == Unit.Cm)
            {
                return CSSUnitsConverter.CmToPX(cssValue.Value);
            }
            else if (cssValue.Unit == Unit.Mm)
            {
                return CSSUnitsConverter.CmToPX(cssValue.Value);
            }
            else if (cssValue.Unit == Unit.In)
            {
                return CSSUnitsConverter.InToPX(cssValue.Value);
            }
            else if (cssValue.Unit == Unit.Pt)
            {
                return CSSUnitsConverter.PtToPX(cssValue.Value);
            }
            else if (cssValue.Unit == Unit.Pc)
            {
                return CSSUnitsConverter.PcToPX(cssValue.Value);
            }
            else if (cssValue.Unit == Unit.Percent)
            {
                return PercentToPx(rule, element);
            }
            else if (cssValue.Unit == Unit.Em)
            {
                return EmToPx(rule, element);
            }

            return 0;
        }

        public static float PercentToPx (Rule rule, DOMElement element)
        {
            if (rule.Property == "width") {
                float parentWidth = (element.Parent != null) ? element.Parent.ComputedStyle.Size.Width : Program.deviceWidth;

                float width = (rule.ComputedValue.ValueBeforeComputing / 100) * parentWidth;

                return width;
            }
            else if (rule.Property == "height")
            {
                float parentHeight = (element.Parent != null) ? element.Parent.Style.Size.Height : 0;

                float height = (rule.ComputedValue.ValueBeforeComputing / 100) * parentHeight;

                return height;
            }

            return 0f;
        }

        public static float EmToPx(Rule rule, DOMElement element)
        {
            float parentFontSize = (element.Parent != null) ? element.Parent.Style.Font.Size : 14; // 14 - Document font-size

            return parentFontSize * rule.ComputedValue.Value;
        }
    }
}
