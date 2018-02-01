using System;
using System.Collections.Generic;

namespace LayoutEngine {
    public class HTMLDocument {
        public List<DOMElement> Children;
        public List<Meta> MetaTags;
        public int Height;
        public int Width;

        public Size getViewport () {
            Size size = new Size(this.Height, this.Width);

            foreach (Meta meta in MetaTags) {
                if (meta.Property == MetaType.ViewportWidth) {
                    size.Width = meta.ComputedValue;
                } else if (meta.Property == MetaType.ViewportHeight) {
                    size.Height = meta.ComputedValue;
                }
            }

            return size;
        }
    }
}
