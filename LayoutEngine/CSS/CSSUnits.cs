using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace LayoutEngine
{
    class CSSUnits
    {
        public static CSSProperty parseCSSProperty(Rule rule)
        {
            CSSProperty cssProperty = new CSSProperty();

            List<string> array = Regex.Split(rule.Value, @"[^0-9\.]+").Where(c => c != "." && c.Trim() != "").ToList<string>();

            if (array.Count > 0 && rule.Value.Length > 2)
            {
                float value = float.Parse(array[0], CultureInfo.InvariantCulture.NumberFormat);
                int startIndex = value.ToString().Length;

                string unitType = rule.Value.Substring(startIndex, rule.Value.Length - startIndex);
                cssProperty.Value = value;

                if (unitType == "px")
                {
                    cssProperty.Unit = Unit.px;
                }
                else if (unitType == "cm")
                {
                    cssProperty.Unit = Unit.cm;
                }
                else if (unitType == "mm")
                {
                    cssProperty.Unit = Unit.mm;
                }

                return cssProperty;
            }

            return null;
        }
 
        public static float convertAnyUnitToPixels(CSSProperty cssProperty)
        {
            if (cssProperty.Unit == Unit.px)
            {
                return cssProperty.Value;
            }
            else if (cssProperty.Unit == Unit.cm)
            {
                return CSSUnitsConverter.cmToPX(cssProperty.Value);
            }
            else if (cssProperty.Unit == Unit.mm)
            {
                return CSSUnitsConverter.mmToPX(cssProperty.Value);
            }

            return -1;
        }
    }
}
