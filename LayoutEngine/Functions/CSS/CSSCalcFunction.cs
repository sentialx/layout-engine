using System;
using System.Data;
using System.Globalization;
using System.Text;

namespace LayoutEngine
{
    class CSSCalcFunction
    {
        public static float Calculate(Rule rule, DOMElement element)
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
                value = Compute(rule, element, value, Unit.Px, false);

                foreach (Unit unit in CSSUnits.allUnits) value = Compute(rule, element, value, unit);

                if (value == null)
                {
                    Console.WriteLine("Calc function isn't correct!");

                    return -1f;
                }
                else
                {
                    return float.Parse(new DataTable().Compute(value, "").ToString().Replace(',', '.'), CultureInfo.InvariantCulture.NumberFormat);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);

                return -1f;
            }
        }

        private static string Compute(Rule rule, DOMElement element, string calcString, Unit unitType, bool convert = true)
        {
            while (true)
            {
                string unit = CSSUnits.GetUnitAbbreviation(unitType);

                int unitStartIndex = calcString.IndexOf(unit);
                if (unitStartIndex == -1) break;
                int unitEndIndex = unitStartIndex + unit.Length;

                // Get starting value index
                int valueStartIndex = GetStartIndex(calcString, unitStartIndex);

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

                float computedValue = (convert) ? CSSUnits.ConvertAnyUnitToPixels(rule, element) : parsedValue;

                // Replace string with computed value
                StringBuilder stringBuilder = new StringBuilder(calcString);
                stringBuilder.Remove(valueStartIndex, unitStartIndex - valueStartIndex + unit.Length);
                stringBuilder.Insert(valueStartIndex, computedValue.ToString().Replace(',', '.'));

                calcString = stringBuilder.ToString();
            }

            return calcString;
        }

        private static int GetStartIndex(string str, int index)
        {
            int startIndex = -1;

            for (int i = index - 1; i >= 0; i--)
            {
                char sign = str[i];
                bool isArithmeticChar = (sign == '+' || sign == '-' || sign == '*' || sign == '/');
                bool isSpace = (sign == ' ');

                if (i == 0 || isSpace || isArithmeticChar)
                {
                    if (isArithmeticChar || isSpace) i++;

                    startIndex = i;
                    break;
                }
            }

            Console.WriteLine(startIndex);

            return startIndex;
        }
    }
}
