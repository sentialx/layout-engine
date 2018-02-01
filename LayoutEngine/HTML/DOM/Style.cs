using System.Drawing;

namespace LayoutEngine {
    public class Style {
        public Color Color = Color.Black;
        public Color BackgroundColor = Color.Transparent;
        public Size Size = new Size(-1, -1);
        public Position Position = new Position(0, 0);
        public Margin Margin = new Margin(0, 0, 0, 0);
        public Padding Padding = new Padding(0, 0, 0, 0);
        public Display Display = Display.Block;
        public Font Font;
        public Border Border = new Border();
    }
}
