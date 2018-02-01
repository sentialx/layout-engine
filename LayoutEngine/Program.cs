using System;
using System.Collections.Generic;
using System.Drawing;

namespace LayoutEngine {
    public class Program {
        static void Main (string[] args) {
            // Minify HTML code before parsing.
            string html = HTML.Minify(System.IO.File.ReadAllLines("index.html"));

            Render(html);

            // Beautify HTML code after rendering and write to console.
            List<string> lines = HTML.Beautify(html);
            foreach (string line in lines) {
                Console.WriteLine(line);
            }

            Console.ReadKey();
        }

        private static void Render (string html) {
            HTMLDocument htmlDocument = HTML.Parse(html);
            htmlDocument.Width = 1366;
            htmlDocument.Height = 768;

            Bitmap bmp = LayoutEngine.Render(htmlDocument);

            bmp.Save("image.jpg");
        }
    }
}
