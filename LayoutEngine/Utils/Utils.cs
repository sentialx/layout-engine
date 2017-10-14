using System.Collections.Generic;

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
    }
}
