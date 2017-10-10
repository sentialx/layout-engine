using System;
using System.Collections.Generic;
using System.Drawing;

namespace LayoutEngine
{
    class Program
    {
        private static Bitmap bmp;

        static void Main(string[] args)
        {
            string html = HTML.Minify(System.IO.File.ReadAllLines("index.html"));

            Render(html);

            List<string> lines = HTML.Beautify(html);

            foreach (string line in lines)
            {
                Console.WriteLine(line);
            }

            Console.ReadKey();
        }

        private static void SetStyles(List<DOMElement> elements, List<Rule> inheritedStyles = null)
        {
            if (inheritedStyles == null) inheritedStyles = new List<Rule>();

            foreach (DOMElement element in elements)
            {
                List<Rule> newInheritedStyles = new List<Rule>();

                string css = System.IO.File.ReadAllText("defaultTags.css");
                List<RuleSet> ruleSets = CSS.Parse(css);

                string fontFamily = "Times New Roman";

                List<FontStyle> fontStyles = new List<FontStyle>();
                List<FontWeight> fontWeights = new List<FontWeight>();
                List<TextDecoration> textDecorations = new List<TextDecoration>();

                float fontSize = 14;
                Color color = Color.Black;

                List<Rule> inheritedStylesCopy = new List<Rule>();
                inheritedStylesCopy.AddRange(inheritedStyles);

                foreach (Rule rule in inheritedStylesCopy)
                {
                    if (rule.Property == "font-weight")
                    {
                        if (rule.Value == "bold")
                        {
                            fontWeights.Add(FontWeight.Bold);
                        }
                        else if (rule.Value == "normal")
                        {
                            fontWeights.Add(FontWeight.Normal);
                        }
                    }
                    else if (rule.Property == "font-style")
                    {
                        if (rule.Value == "italic")
                        {
                            fontStyles.Add(FontStyle.Italic);
                        }
                        else if (rule.Value == "normal")
                        {
                            fontStyles.Add(FontStyle.Normal);
                        }
                    } else if (rule.Property == "font-size")
                    {
                        fontSize = float.Parse(rule.Value.Split(new string[] { "px" }, StringSplitOptions.None)[0]);
                    } else if (rule.Property == "font-family")
                    {
                        fontFamily = rule.Value;
                    } else if (rule.Property == "color")
                    {
                        color = ColorTranslator.FromHtml(rule.Value);
                        element.Style.Color = color;
                    }
                    else if (rule.Property == "text-decoration")
                    {
                        if (rule.Value == "underline")
                        {
                            textDecorations.Add(TextDecoration.Underline);
                        }
                    } else if (rule.Property == "padding-top")
                    {
                        float paddingTop = float.Parse(rule.Value.Split(new string[] { "px" }, StringSplitOptions.None)[0]);
                        element.ComputedStyle.Padding.Top = paddingTop;

                        inheritedStyles.Remove(rule);
                    } else if (rule.Property == "padding-left")
                    {
                        float paddingLeft = float.Parse(rule.Value.Split(new string[] { "px" }, StringSplitOptions.None)[0]);
                        element.ComputedStyle.Padding.Left = paddingLeft;

                        inheritedStyles.Remove(rule);
                    }
                }
                
                if (element.Type == DOMElementType.Normal)
                {
                    string selector = element.Tag.Name;

                    foreach (RuleSet ruleSet in ruleSets)
                    {
                        if (ruleSet.Selector == selector)
                        {
                            foreach (Rule rule in ruleSet.Rules)
                            {
                                if (rule.Property == "font-weight")
                                {
                                    if (rule.Value == "bold")
                                    {
                                        fontWeights.Add(FontWeight.Bold);
                                    }
                                    else if (rule.Value == "normal")
                                    {
                                        fontWeights.Add(FontWeight.Normal);
                                    }

                                    newInheritedStyles.Add(rule);
                                }
                                else if (rule.Property == "font-style")
                                {
                                    if (rule.Value == "italic")
                                    {
                                        fontStyles.Add(FontStyle.Italic);
                                    }
                                    else if (rule.Value == "normal")
                                    {
                                        fontStyles.Add(FontStyle.Normal);
                                    }

                                    newInheritedStyles.Add(rule);
                                }
                                else if (rule.Property == "font-size")
                                {
                                    fontSize = float.Parse(rule.Value.Split(new string[] { "px" }, StringSplitOptions.None)[0]);

                                    newInheritedStyles.Add(rule);
                                }
                                else if (rule.Property == "font-family")
                                {
                                    fontFamily = rule.Value;

                                    newInheritedStyles.Add(rule);
                                }
                                else if (rule.Property == "color")
                                {
                                    color = ColorTranslator.FromHtml(rule.Value);
                                    element.Style.Color = color;

                                    newInheritedStyles.Add(rule);
                                }
                                else if (rule.Property == "text-decoration")
                                {
                                    if (rule.Value == "underline")
                                    {
                                        textDecorations.Add(TextDecoration.Underline);
                                    }

                                    newInheritedStyles.Add(rule);
                                }
                                else if (rule.Property == "padding-top")
                                {
                                    element.Style.Padding.Top = float.Parse(rule.Value.Split(new string[] { "px" }, StringSplitOptions.None)[0]);

                                    newInheritedStyles.Add(rule);
                                }
                                else if (rule.Property == "padding-left")
                                {
                                    element.Style.Padding.Left = float.Parse(rule.Value.Split(new string[] { "px" }, StringSplitOptions.None)[0]);

                                    newInheritedStyles.Add(rule);
                                }
                                else if (rule.Property == "padding-right")
                                {
                                    element.Style.Padding.Right = float.Parse(rule.Value.Split(new string[] { "px" }, StringSplitOptions.None)[0]);
                                }
                                else if (rule.Property == "padding-bottom")
                                {
                                    element.Style.Padding.Bottom = float.Parse(rule.Value.Split(new string[] { "px" }, StringSplitOptions.None)[0]);
                                }
                                else if (rule.Property == "margin-top")
                                {
                                    element.Style.Margin.Top = float.Parse(rule.Value.Split(new string[] { "px" }, StringSplitOptions.None)[0]);
                                }
                                else if (rule.Property == "margin-bottom")
                                {
                                    element.Style.Margin.Bottom = float.Parse(rule.Value.Split(new string[] { "px" }, StringSplitOptions.None)[0]);
                                }
                                else if (rule.Property == "margin-left")
                                {
                                    element.Style.Margin.Left = float.Parse(rule.Value.Split(new string[] { "px" }, StringSplitOptions.None)[0]);
                                }
                                else if (rule.Property == "margin-right")
                                {
                                    element.Style.Margin.Right = float.Parse(rule.Value.Split(new string[] { "px" }, StringSplitOptions.None)[0]);
                                }
                                else if (rule.Property == "height")
                                {
                                    element.Style.Size.Height = float.Parse(rule.Value.Split(new string[] { "px" }, StringSplitOptions.None)[0]);
                                }
                                else if (rule.Property == "width")
                                {
                                    element.Style.Size.Width = float.Parse(rule.Value.Split(new string[] { "px" }, StringSplitOptions.None)[0]);
                                }
                                else if (rule.Property == "background-color")
                                {
                                    element.Style.BackgroundColor = ColorTranslator.FromHtml(rule.Value);
                                }
                                else if (rule.Property == "border")
                                {
                                    float borderWidth = float.Parse(rule.Value.Split(' ')[0].Split(new string[] { "px" }, StringSplitOptions.None)[0]);
                                    string borderType = rule.Value.Split(' ')[1];
                                    Color borderColor = ColorTranslator.FromHtml(rule.Value.Split(' ')[2]);

                                    element.Style.Border = new Border(borderWidth, BorderType.Solid, borderColor);
                                }
                                else if (rule.Property == "display")
                                {
                                    if (rule.Value == "none")
                                    {
                                        element.Style.Display = Display.None;
                                    } else if (rule.Value == "block")
                                    {
                                        element.Style.Display = Display.Block;
                                    } else if (rule.Value == "initial")
                                    {
                                        element.Style.Display = Display.Initial;
                                    } else if (rule.Value == "inherit")
                                    {
                                        element.Style.Display = Display.Inherit;
                                    } else if (rule.Value == "flex")
                                    {
                                        element.Style.Display = Display.Flex;
                                    } else if (rule.Value == "inline-flex")
                                    {
                                        element.Style.Display = Display.InlineFlex;
                                    } else if (rule.Value == "inline-table")
                                    {
                                        element.Style.Display = Display.InlineTable;
                                    } else if (rule.Value == "inline")
                                    {
                                        element.Style.Display = Display.Inline;
                                    } else if (rule.Value == "inline-block")
                                    {
                                        element.Style.Display = Display.InlineBlock;
                                    } else if (rule.Value == "list-item")
                                    {
                                        element.Style.Display = Display.ListItem;
                                    } else if (rule.Value == "run-in")
                                    {
                                        element.Style.Display = Display.RunIn;
                                    } else if (rule.Value == "table")
                                    {
                                        element.Style.Display = Display.Table;
                                    } else if (rule.Value == "table-cell")
                                    {
                                        element.Style.Display = Display.TableCell;
                                    } else if (rule.Value == "table-row")
                                    {
                                        element.Style.Display = Display.TableRow;
                                    } else if (rule.Value == "table-caption")
                                    {
                                        element.Style.Display = Display.TableCaption;
                                    } else if (rule.Value == "table-column-group")
                                    {
                                        element.Style.Display = Display.TableColumnGroup;
                                    } else if (rule.Value == "table-header-group")
                                    {
                                        element.Style.Display = Display.TableHeaderGroup;
                                    } else if (rule.Value == "table-footer-group")
                                    {
                                        element.Style.Display = Display.TableFooterGroup;
                                    } else if (rule.Value == "table-row-group")
                                    {
                                        element.Style.Display = Display.TableRowGroup;
                                    } else if (rule.Value == "table-column")
                                    {
                                        element.Style.Display = Display.TableColumn;
                                    }
                                }
                            }
                        }
                    }
                }

                Font font = new Font(new FontFamily(fontFamily), fontSize, System.Drawing.FontStyle.Regular, GraphicsUnit.Pixel);

                foreach (FontStyle fontStyle in fontStyles)
                {
                    if (fontStyle == FontStyle.Italic)
                    {
                        font = new Font(font, font.Style ^ System.Drawing.FontStyle.Italic);
                    }   
                }

                foreach (FontWeight fontWeight in fontWeights)
                {
                    if (fontWeight == FontWeight.Bold)
                    {
                        font = new Font(font, font.Style ^ System.Drawing.FontStyle.Bold);
                    }
                    else if (fontWeight == FontWeight.Normal) 
                    {
                        font = new Font(font, font.Style ^ System.Drawing.FontStyle.Regular);
                    }
                }

                foreach (TextDecoration textDecoration in textDecorations)
                {
                    if (textDecoration == TextDecoration.Underline)
                    {
                        font = new Font(font, font.Style ^ System.Drawing.FontStyle.Underline);
                    }
                }

                element.Style.Font = font;

                foreach (Rule rule in inheritedStyles)
                {
                    newInheritedStyles.Add(rule);
                }

                if (element.Children.Count > 0)
                {
                    SetStyles(element.Children, newInheritedStyles);
                }
            }
        }

        private static Size GetChildrenSizes (DOMElement parent)
        {
            Size childrenSize = new Size(0, 0);

            foreach (DOMElement child in parent.Children)
            {
                Style currStyle = child.Style;
                ComputedStyle currCompStyle = child.ComputedStyle;

                if (currStyle.Display == Display.Block || currStyle.Display == Display.Table || currStyle.Display == Display.TableRow)
                {
                    childrenSize.Height += currCompStyle.Size.Height;
                    if (currCompStyle.Size.Width > childrenSize.Width) childrenSize.Width = currCompStyle.Size.Width;
                }
                else if (currStyle.Display == Display.InlineBlock || currStyle.Display == Display.TableCell)
                {
                    if (parent.Children.IndexOf(child) > 0)
                    {
                        Style prevStyle = parent.Children[parent.Children.IndexOf(child) - 1].Style;
                        if (prevStyle.Display == currStyle.Display)
                        {
                            if (currCompStyle.Size.Height > childrenSize.Height) childrenSize.Height = currCompStyle.Size.Height;
                        }
                        else
                        {
                            childrenSize.Height += currCompStyle.Size.Height;
                        }
                    }
                    else
                    {
                        childrenSize.Height += currCompStyle.Size.Height;
                    }
                    childrenSize.Width += currCompStyle.Size.Width;
                }
            }

            return childrenSize;
        }

        private static void SetTableSizes (DOMElement element)
        {
            foreach (DOMElement child in element.Children)
            {
                if (child.Style.Display == Display.TableRow)
                {
                    child.ComputedStyle.Size.Width = 0;

                    foreach (DOMElement child1 in element.Children)
                    {
                        if (child1.Style.Display == Display.TableRow && child1 != child)
                        {
                            for (int i = 0; i < child1.Children.Count; i++)
                            {
                                if (!(i > child.Children.Count - 1) && !(i > child1.Children.Count - 1))
                                {
                                    if (child.Children[i].ComputedStyle.Size.Width < child1.Children[i].ComputedStyle.Size.Width)
                                    {
                                        child.Children[i].ComputedStyle.Size.Width = child1.Children[i].ComputedStyle.Size.Width;
                                    }
                                }
                            }
                        }
                    }

                    foreach (DOMElement tableCell in child.Children)
                    {
                        child.ComputedStyle.Size.Width += tableCell.ComputedStyle.Size.Width;
                    }

                    float maxWidth = 0;

                    foreach (DOMElement child1 in element.Children)
                    {
                        if (child1.ComputedStyle.Size.Width > maxWidth) maxWidth = child1.ComputedStyle.Size.Width;
                    }
                    child.ComputedStyle.Size.Width = maxWidth;
                }
            }
        }

        private static void SetSizesWithPaddings (DOMElement element, Size childrenSizes = new Size())
        {
            float height = element.ComputedStyle.Size.Height;
            float width = element.ComputedStyle.Size.Width;

            Padding padding = element.Style.Padding;
            Size size = element.ComputedStyle.Size;

            if (padding.Top + childrenSizes.Height >= size.Height)
            {
                height += padding.Top;
            }

            if (padding.Left + childrenSizes.Width >= size.Width)
            {
                width += padding.Left;
            }

            if (padding.Left + padding.Right + childrenSizes.Width >= size.Width)
            {
                width += padding.Right;
            }

            if (padding.Top + padding.Bottom + childrenSizes.Height >= size.Height)
            {
                height += padding.Bottom;
            }

            element.ComputedStyle.Size.Width = width;
            element.ComputedStyle.Size.Height = height;
        }

        private static void SetSizes (DOMElement element, Size childrenSizes = new Size())
        {
            Size newChildrenSizes = new Size(0, 0);

            if (element.Type == DOMElementType.Normal)
            {
                element.ComputedStyle.Size.Width = (element.Style.Size.Width != -1) ? element.Style.Size.Width : childrenSizes.Width;
                element.ComputedStyle.Size.Height = (element.Style.Size.Height != -1) ? element.Style.Size.Height : childrenSizes.Height;
            } else if (element.Type == DOMElementType.Text)
            {
                Font font = element.Style.Font;

                using (Graphics graphics = Graphics.FromImage(bmp))
                {
                    float textWidth = graphics.MeasureString(element.Content, font).Width;
                    float textHeight = graphics.MeasureString(element.Content, font).Height;

                    element.ComputedStyle.Size.Height = (element.Style.Size.Height == -1) ? textHeight - 2 : element.Style.Size.Height;
                    element.ComputedStyle.Size.Width = (element.Style.Size.Width == -1) ? textWidth : element.Style.Size.Width;
                }
            }

            if (element.Parent != null)
            {
                newChildrenSizes = GetChildrenSizes(element.Parent);
            }

            if (element.Style.Display == Display.Table)
            {
                SetTableSizes(element);
            }

            SetSizesWithPaddings(element, childrenSizes);

            if (element.Parent != null)
            {
                SetSizes(element.Parent, newChildrenSizes);
            }
        }

        private static void SetSizes(List<DOMElement> elements)
        {
            foreach (DOMElement element in elements)
            {
                if (element.Children.Count > 0)
                {
                    SetSizes(element.Children);
                }
                else
                {
                    SetSizes(element);
                }
            }
        }

        private static void SetPositions(List<DOMElement> elements)
        {
            foreach (DOMElement element in elements)
            {
                if (elements.IndexOf(element) > 0)
                {
                    DOMElement prevElement = elements[elements.IndexOf(element) - 1];
                    if (element.Style.Display == Display.InlineBlock || element.Style.Display == Display.TableCell)
                    {
                        if (prevElement.Style.Display == element.Style.Display)
                        {
                            element.ComputedStyle.Position.Y = prevElement.ComputedStyle.Position.Y;
                            element.ComputedStyle.Position.X = prevElement.ComputedStyle.Position.X + prevElement.ComputedStyle.Size.Width;
                        } else
                        {
                            element.ComputedStyle.Position.Y = prevElement.ComputedStyle.Position.Y + prevElement.ComputedStyle.Size.Height + prevElement.Style.Margin.Bottom + element.Style.Margin.Top;
                        }

                        if (prevElement.Style.Margin.Right >= element.Style.Margin.Left)
                        {
                            element.ComputedStyle.Position.X += prevElement.Style.Margin.Right;
                        } else
                        {
                            element.ComputedStyle.Position.X += element.Style.Margin.Left;
                        }
                        element.ComputedStyle.Position.Y += element.ComputedStyle.Padding.Top;
                    } else if (element.Style.Display == Display.Block || element.Style.Display == Display.TableRow || element.Style.Display == Display.Table)
                    {
                        element.ComputedStyle.Position.Y = prevElement.ComputedStyle.Position.Y + prevElement.ComputedStyle.Size.Height;
                        if (element.Parent != null)
                        {
                            element.ComputedStyle.Position.X = element.Parent.ComputedStyle.Position.X;
                        }

                        if (prevElement.Style.Margin.Bottom >= element.Style.Margin.Top)
                        {
                            element.ComputedStyle.Position.Y += prevElement.Style.Margin.Bottom;
                        } else
                        {
                            element.ComputedStyle.Position.Y += element.Style.Margin.Top;
                        }
                        if (prevElement.Style.Margin.Right >= element.Style.Margin.Left)
                        {
                            element.ComputedStyle.Position.X += prevElement.Style.Margin.Right;
                        }
                        else
                        {
                            element.ComputedStyle.Position.X += element.Style.Margin.Left;
                        }
                        element.ComputedStyle.Position.X += element.ComputedStyle.Padding.Left;
                    }
                }
                else
                {
                    if (element.Parent != null)
                    {
                        element.ComputedStyle.Position.Y = element.Parent.ComputedStyle.Position.Y + element.Style.Margin.Top;
                        element.ComputedStyle.Position.X = element.Parent.ComputedStyle.Position.X + element.Style.Margin.Left;
                    } else
                    {
                        element.ComputedStyle.Position.Y = element.Style.Margin.Top;
                        element.ComputedStyle.Position.X = element.Style.Margin.Left;
                    }
                    element.ComputedStyle.Position.Y += element.ComputedStyle.Padding.Top;
                    
                    element.ComputedStyle.Position.X += element.ComputedStyle.Padding.Left;
                }

                if (element.Children.Count > 0)
                {
                    SetPositions(element.Children);
                }
            }
        }

        private static void Reflow (List<DOMElement> elements)
        {
            SetStyles(elements);

            SetSizes(elements);

            SetPositions(elements);
        }

        private static void Render (List<DOMElement> elements)
        {
            foreach (DOMElement element in elements)
            {
                using (Graphics graphics = Graphics.FromImage(bmp))
                {
                    graphics.PageUnit = GraphicsUnit.Pixel;

                    if (element.Style.BackgroundColor != Color.Transparent)
                    {
                        graphics.FillRectangle(new SolidBrush(element.Style.BackgroundColor), element.ComputedStyle.Position.X, element.ComputedStyle.Position.Y, element.ComputedStyle.Size.Width, element.ComputedStyle.Size.Height);
                    }

                    graphics.DrawRectangle(new Pen(new SolidBrush(element.Style.Border.Color), element.Style.Border.Width), element.ComputedStyle.Position.X, element.ComputedStyle.Position.Y, element.ComputedStyle.Size.Width, element.ComputedStyle.Size.Height);

                    if (element.Type == DOMElementType.Text)
                    {
                        graphics.DrawString(element.Content, element.Style.Font, new SolidBrush(element.Style.Color), element.ComputedStyle.Position.X, element.ComputedStyle.Position.Y);
                    }

                    graphics.Flush();
                    graphics.Dispose();
                }

                if (element.Children.Count > 0)
                {
                    Render(element.Children);
                }
            }
        }

        private static void Render (string html)
        {
            HTMLDocument document = HTML.Parse(html);

            List<DOMElement> elements = document.Children;

            bmp = new Bitmap(1366, 768);

            using (Graphics graphics = Graphics.FromImage(bmp))
            {
                graphics.FillRectangle(new SolidBrush(Color.White), 0, 0, bmp.Width, bmp.Height);
                graphics.Flush();
                graphics.Dispose();
            }

            Reflow(elements);

            Render(elements);

            bmp.Save("image.jpg");
        }
    }
}
