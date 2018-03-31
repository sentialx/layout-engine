# layout-engine
A simple layout engine that renders HTML and CSS.

# Testing
To test it out, simply create an `index.html` file in `layout-engine/bin/Debug` folder and `styles.css` in `layout-engine/bin/Debug/CSS` folder.

## Limitations
* Only valid HTML code works. 

  Bad:
  ```html
  <html>
    <body>
      <div>
        <div>
          Test
    </body>
  </html>
  
  ```
  Good:
  ```html
  <html>
    <body>
      <div>
        <div>
          Test
        </div>
      </div>
    </body>
  </html>
  ```
  
* In CSS only hex colors are accepted.
* The `padding` rule doesn't work. Instead use `padding-left`, `padding-top` etc. The same as `margin`.
* For borders you can use only `border: {width} solid {color}`.
* Not all units work.
