﻿using System.Collections.Generic;
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

                string unitType = rule.Value.Substring(startIndex, rule.Value.Length - startIndex).ToLower();
                cssProperty.Value = value;

                if (unitType == "px")
                {
                    cssProperty.Unit = Unit.Px;
                }
                else if (unitType == "cm")
                {
                    cssProperty.Unit = Unit.Cm;
                }
                else if (unitType == "mm")
                {
                    cssProperty.Unit = Unit.Mm;
                }
                else if (unitType == "in")
                {
                    cssProperty.Unit = Unit.In;
                }
                else if (unitType == "pt")
                {
                    cssProperty.Unit = Unit.Pt;
                }

                return cssProperty;
            }

            return null;
        }
 
        public static float convertAnyUnitToPixels(CSSProperty cssProperty)
        {
            if (cssProperty.Unit == Unit.Px)
            {
                return cssProperty.Value;
            }
            else if (cssProperty.Unit == Unit.Cm)
            {
                return CSSUnitsConverter.cmToPX(cssProperty.Value);
            }
            else if (cssProperty.Unit == Unit.Mm)
            {
                return CSSUnitsConverter.mmToPX(cssProperty.Value);
            }
            else if (cssProperty.Unit == Unit.In)
            {
                return CSSUnitsConverter.inToPX(cssProperty.Value);
            }
            else if (cssProperty.Unit == Unit.Pt)
            {
                return CSSUnitsConverter.ptToPX(cssProperty.Value);
            }

            return -1;
        }
    }
}