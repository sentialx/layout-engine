using System.Collections.Generic;

namespace LayoutEngine
{
    public class HTMLTag
    {
        public string Name;
        public string Code;
        public TagType Type;
        public List<HTMLAttribute> Attributes = new List<HTMLAttribute>();
    }
}
