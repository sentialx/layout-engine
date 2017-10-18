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
            string value = rule.Value.Split(new[] { "calc(" }, StringSplitOptions.None)[1];

            if (value[value.Length - 1] != ')')
            {
               return -1f; 
            } else
            {
                value = value.Substring(0, value.Length - 1);
            }

            try
            {
                // Convert all percentes to pixels
                while (true)
                {
                    int percentIndex = value.IndexOf("%");
                    if (percentIndex == -1) break;

                    int startIndex = getStartIndex(value, percentIndex);

                    // Get and parse value before computing
                    string val = value.Substring(startIndex, percentIndex - startIndex);
                    float valueBeforeComputing = float.Parse(val, CultureInfo.InvariantCulture.NumberFormat);

                    // Convert percent to pixels
                    rule.ComputedValue.ValueBeforeComputing = valueBeforeComputing;
                    float computedValueFromPercent = PercentToPx(rule, element);

                    // Replace percent value with computed value (pixels)
                    int length = val.Length + 1;

                    StringBuilder stringBuilder = new StringBuilder(value);
                    stringBuilder.Remove(startIndex, length);
                    stringBuilder.Insert(startIndex, computedValueFromPercent.ToString().Replace(',', '.'));
                    value = stringBuilder.ToString();
                }
                
                // Calculate
                DataTable dt = new DataTable();
                float computedValue = float.Parse(dt.Compute(value, "").ToString().Replace(',', '.'), CultureInfo.InvariantCulture.NumberFormat);

                return computedValue;
            } catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return -1f;
            }
        }

        private static int getStartIndex (string str, int index)
        {
            int startIndex = -1;

            for (int i = index - 1; i >= 0; i--)
            {
                char sign = str[i];

                if (i == 0 || sign == ' ' || sign == '+' || sign == '-' || sign == '*' || sign == '/')
                {
                    startIndex = i;
                    break;
                }
            }

            return startIndex;
        }
    }
}
