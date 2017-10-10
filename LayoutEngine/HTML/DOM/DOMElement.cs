using System.Collections.Generic;

namespace LayoutEngine
{
    public class DOMElement
    {
        public string Content = "";
        public int Level;
        public List<DOMElement> Children = new List<DOMElement>();
        public DOMElement Parent;
        public HTMLTag Tag;
        public DOMElementType Type;
        public Style Style = new Style();
        public ComputedStyle ComputedStyle = new ComputedStyle();
    }
}
