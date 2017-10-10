namespace LayoutEngine
{
    public struct Margin
    {
        public float Top;
        public float Right;
        public float Bottom;
        public float Left;

        public Margin (float top, float right, float bottom, float left)
        {
            this.Top = top;
            this.Right = right;
            this.Bottom = bottom;
            this.Left = left;
        }
    }
}
