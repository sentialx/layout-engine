using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LayoutEngine
{
    public class CSSUnits
    {
        public static CSSValue ParseValue (Rule rule, DOMElement element)
        {
            CSSValue cssValue = new CSSValue();

            if (rule.Value.StartsWith("calc("))
            {
                cssValue.Unit = Unit.CalcFunc;
                rule.ComputedValue = cssValue;

                return cssValue;
            } else {
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
                return CSSUnitsConverter.MmToPX(cssValue.Value);
            }
            else if (cssValue.Unit == Unit.In)
            {
                return CSSUnitsConverter.InToPX(cssValue.Value);
            }
            else if (cssValue.Unit == Unit.Percent)
            {
                return PercentToPx(rule, element);
            }
            else if (cssValue.Unit == Unit.Pt)
            {
                return CSSUnitsConverter.PtToPX(cssValue.Value);
            }
            else if (cssValue.Unit == Unit.Em)
            {
                return EmToPx(rule, element);
            }

            return -1f;
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

        public static float CalcFunction (Rule rule, DOMElement element)
        {
            try
            {
                // Cut unnecessary parts
                string value = rule.Value.Split(new[] { "calc(" }, StringSplitOptions.None)[1];
                value = value.ToLower();

                if (value[value.Length - 1] != ')') return -1f;
                else value = value.Substring(0, value.Length - 1);

                if (value.Length < 2) return -1f;

                // Cut pixel unit
                value = CalcFunctionCompute(rule, element, value, Unit.Px, false);

                Unit[] units = new Unit[] {
                    Unit.Cm,
                    Unit.Em,
                    Unit.In,
                    Unit.Mm,
                    Unit.Pc,
                    Unit.Percent,
                    Unit.Pt
                };

                foreach (Unit unit in units) value = CalcFunctionCompute(rule, element, value, unit);

                if (value == null)
                {
                    Console.WriteLine("Calc function insn't correct!");

                    return -1f;
                }
                else
                {
                    float computedValue = float.Parse(new DataTable().Compute(value, "").ToString().Replace(',', '.'), CultureInfo.InvariantCulture.NumberFormat);

                    return computedValue;
                }
            } catch (Exception exception)
            {
                Console.WriteLine(exception.Message);

                return -1f;
            }
        }

        private static string CalcFunctionCompute (Rule rule, DOMElement element, string calcString, Unit unitType, bool convert = true)
        {
            while (true)
            {
                string unit = GetUnitAbbreviation(unitType);

                int unitStartIndex = calcString.IndexOf(unit);
                if (unitStartIndex == -1) break;
                int unitEndIndex = unitStartIndex + unit.Length;

                // Get starting value index
                int valueStartIndex = CalcFunctionGetStartIndex(calcString, unitStartIndex);

                if (valueStartIndex == unitStartIndex) return null;

                // Get and parse value
                string value = calcString.Substring(valueStartIndex, unitStartIndex - valueStartIndex);
                float parsedValue = float.Parse(value, CultureInfo.InvariantCulture.NumberFormat);

                // Compute the value
                if (convert)
                {
                    rule.ComputedValue.Unit = unitType;
                    rule.ComputedValue.ValueBeforeComputing = parsedValue;
                    rule.ComputedValue.Value = parsedValue;
                }

                float computedValue = (convert) ? ConvertAnyUnitToPixels(rule, element) : parsedValue;

                // Replace string with computed value
                StringBuilder stringBuilder = new StringBuilder(calcString);
                stringBuilder.Remove(valueStartIndex, unitStartIndex - valueStartIndex + unit.Length);
                stringBuilder.Insert(valueStartIndex, computedValue.ToString().Replace(',', '.'));

                calcString = stringBuilder.ToString();
            }

            return calcString;
        }

        private static int CalcFunctionGetStartIndex (string str, int index)
        {
            int startIndex = -1;

            for (int i = index - 1; i >= 0; i--)
            {
                char sign = str[i];
                bool isArithmeticSign = (sign == '+' || sign == '-' || sign == '*' || sign == '/');
                bool isSpace = (sign == ' ');

                if (i == 0 || isSpace || isArithmeticSign)
                {
                    if (isArithmeticSign || isSpace) i++;

                    startIndex = i;
                    break;
                }
            }

            return startIndex;
        }

        public static string GetUnitAbbreviation (Unit unit)
        {
            if (unit == Unit.Px) return "px";
            else if (unit == Unit.Cm) return "cm";
            else if (unit == Unit.Mm) return "mm";
            else if (unit == Unit.In) return "in";
            else if (unit == Unit.Pt) return "pt";
            else if (unit == Unit.Pc) return "pc";
            else if (unit == Unit.Percent) return "%";
            else if (unit == Unit.Em) return "em";

            return null;
        }
    }
}