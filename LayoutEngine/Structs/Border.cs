using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LayoutEngine
{
    public struct Border
    {
        public float Width;
        public BorderType Type;
        public Color Color;

        public Border (float width, BorderType type, Color color)
        {
            this.Width = width;
            this.Type = type;
            this.Color = color;
        }
    }
}
