# layout-engine
A simple layout engine written in C# (it will be ported to C++).

# Testing
To test it out if it works, simply create an index.html and style.css files in Debug folder, and you can code.
For this moment only valid HTML works, so please don't test invalid HTML code. In CSS you can use only hex colors, and `px` as units.
The `padding` rule doesn't work. You can use only `padding-left`, `padding-top`, `padding-right` and `padding-bottom`. The same as `margin`.
For borders you can use only this scheme:
`border: {width} solid {color}`.
