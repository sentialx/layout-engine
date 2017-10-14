using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LayoutEngine
{
    class Utils
    {
        public static List<int> getIndexes(string parent, string str)
        {
            List<int> indexes = new List<int>();
            int i = -1;

            while ((i = parent.IndexOf(str, i + 1)) != -1)
            {
                indexes.Add(i);
            }

            return indexes;
        }

        public static CSSProperty parseCSSProperty (Rule rule)
        {
            CSSProperty cssProperty = new CSSProperty();

            List<string> array = Regex.Split(rule.Value, @"[^0-9\.]+").Where(c => c != "." && c.Trim() != "").ToList<string>();

            if (array.Count > 0 && rule.Value.Length > 2) {
                double value = double.Parse(array[0]);
                string unitType = rule.Value.Substring((int)value - 1, rule.Value.Length - 1);

                cssProperty.Value = value;

                if (unitType == "cm")
                {
                    cssProperty.Unit = Unit.cm;
                }
                                
                return cssProperty;
            }

            return null;
        }
    }
}
