namespace LayoutEngine
{
    public enum Unit
    {
        None,
        Em,      // Relative to the font-size of the element
        Vh,      // Relative to 1% of the height of the viewport
        Vw,      // Relative to 1% of the width of the viewport
        Percent, // Percent
        Px,      // Pixels
        Cm,      // Centimeters
        Mm,      // Millimeters
        In,      // Inches
        Pt,      // Points
        Pc,      // Picas
        Deg,     // Degrees
        Grad,    // Gradians
        Rad,     // Radians
        Turn,    // Turns
        CalcFunc // Calc function
    }
}