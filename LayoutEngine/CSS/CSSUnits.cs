using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace LayoutEngine {
    public class CSSUnits {
        public static List<Unit> allUnits = new List<Unit>()
        {
            Unit.Px,
            Unit.Cm,
            Unit.Mm,
            Unit.In,
            Unit.Pt,
            Unit.Pc,
            Unit.Percent,
            Unit.Em,
            Unit.Vh,
            Unit.Vw
        };

        public static CSSValue ParseValue (Rule rule, DOMElement element) {
            CSSValue cssValue = new CSSValue();

            if (rule.Value.StartsWith("calc(")) {
                cssValue.Unit = Unit.CalcFunc;
                rule.ComputedValue = cssValue;

                return cssValue;
            } else {
                List<string> array = Regex.Split(rule.Value, @"[^0-9\.]+").Where(c => c != "." && c.Trim() != "").ToList();

                if (array.Count > 0 && rule.Value.Length > 2) {
                    float value = float.Parse(array[0], CultureInfo.InvariantCulture.NumberFormat);
                    int startIndex = value.ToString().Length;

                    string unitType = rule.Value.Substring(startIndex, rule.Value.Length - startIndex).ToLower();
                    cssValue.Value = value;

                    foreach (Unit unit in allUnits) {
                        string abbreviation = GetUnitAbbreviation(unit);

                        if (unitType == abbreviation) {
                            cssValue.Unit = unit;
                        }
                    }

                    rule.ComputedValue = cssValue;

                    cssValue.Value = ConvertAnyUnitToPixels(rule, element);
                    cssValue.ValueBeforeComputing = value;

                    return cssValue;
                }
            }

            return null;
        }

        public static float ConvertAnyUnitToPixels (Rule rule, DOMElement element) {
            CSSValue cssValue = rule.ComputedValue;

            if (cssValue.Unit == Unit.Px) {
                return cssValue.Value;
            } else if (cssValue.Unit == Unit.Cm) {
                return CSSUnitsConverter.CmToPX(cssValue.Value);
            } else if (cssValue.Unit == Unit.Mm) {
                return CSSUnitsConverter.MmToPX(cssValue.Value);
            } else if (cssValue.Unit == Unit.In) {
                return CSSUnitsConverter.InToPX(cssValue.Value);
            } else if (cssValue.Unit == Unit.Percent) {
                return CSSUnitsConverter.PercentToPx(rule, element);
            } else if (cssValue.Unit == Unit.Pt) {
                return CSSUnitsConverter.PtToPX(cssValue.Value);
            } else if (cssValue.Unit == Unit.Em) {
                return CSSUnitsConverter.EmToPx(rule, element);
            } else if (cssValue.Unit == Unit.Vh) {
                return CSSUnitsConverter.VhToPx(rule, element);
            } else if (cssValue.Unit == Unit.Vw) {
                return CSSUnitsConverter.VwToPx(rule, element);
            }

            return -1f;
        }

        public static string GetUnitAbbreviation (Unit unit) {
            if (unit == Unit.Px) return "px";
            else if (unit == Unit.Cm) return "cm";
            else if (unit == Unit.Mm) return "mm";
            else if (unit == Unit.In) return "in";
            else if (unit == Unit.Pt) return "pt";
            else if (unit == Unit.Pc) return "pc";
            else if (unit == Unit.Percent) return "%";
            else if (unit == Unit.Em) return "em";
            else if (unit == Unit.Vh) return "vh";
            else if (unit == Unit.Vw) return "vw";

            return null;
        }
    }
}
