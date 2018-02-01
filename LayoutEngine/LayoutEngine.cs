using System;
using System.Collections.Generic;
using System.Drawing;

namespace LayoutEngine {
    public class LayoutEngine {
        public static HTMLDocument htmlDocument;

        private static Bitmap bmp;

        public static Bitmap Render (HTMLDocument htmlDocument) {
            LayoutEngine.htmlDocument = htmlDocument;
            bmp = new Bitmap(htmlDocument.Width, htmlDocument.Height);
            List<DOMElement> elements = htmlDocument.Children;

            using (Graphics graphics = Graphics.FromImage(bmp)) {
                graphics.FillRectangle(new SolidBrush(Color.White), 0, 0, bmp.Width, bmp.Height);
                graphics.Flush();
                graphics.Dispose();
            }

            Reflow(elements);
            Render(elements);

            return bmp;
        }

        private static void Render (List<DOMElement> elements) {
            foreach (DOMElement element in elements) {
                if (element.Style.Display != Display.None) {
                    using (Graphics graphics = Graphics.FromImage(bmp)) {
                        graphics.PageUnit = GraphicsUnit.Pixel;

                        Position position = element.ComputedStyle.Position;
                        Size size = element.ComputedStyle.Size;
                        Border border = element.Style.Border;

                        if (element.Style.BackgroundColor != Color.Transparent) {
                            graphics.FillRectangle(new SolidBrush(element.Style.BackgroundColor), position.X, position.Y, size.Width, size.Height);
                        }

                        graphics.DrawRectangle(new Pen(new SolidBrush(border.Color), border.Width), position.X, position.Y, size.Width, size.Height);

                        if (element.Type == DOMElementType.Text) {
                            graphics.DrawString(element.Content, element.Style.Font, new SolidBrush(element.Style.Color), position.X, position.Y);
                        }

                        graphics.Flush();
                        graphics.Dispose();
                    }

                    if (element.Children.Count > 0) Render(element.Children);
                }
            }
        }

        private static void Reflow (List<DOMElement> elements) {
            List<RuleSet> ruleSets = new List<RuleSet>();

            if (System.IO.File.Exists("CSS/defaultStyles.css")) {
                string defaultStyles = System.IO.File.ReadAllText("CSS/defaultStyles.css");
                ruleSets = CSS.Parse(defaultStyles);
            }

            if (System.IO.File.Exists("CSS/styles.css")) {
                string style = System.IO.File.ReadAllText("CSS/styles.css");
                List<RuleSet> ruleSets2 = CSS.Parse(style);

                foreach (RuleSet rs in ruleSets2) {
                    ruleSets.Add(rs);
                }
            }

            SetFonts(elements, ruleSets);
            SetStyles(elements, ruleSets);

            SetSizes(elements);
            SetPercentSizes(elements);

            SetPositions(elements);
        }

        private static void SetFonts (List<DOMElement> elements, List<RuleSet> ruleSets, List<Rule> inheritedStyles = null) {
            if (inheritedStyles == null) inheritedStyles = new List<Rule>();

            foreach (DOMElement element in elements) {
                List<Rule> newInheritedStyles = new List<Rule>();

                string fontFamily = "Times New Roman";

                List<FontStyle> fontStyles = new List<FontStyle>();
                List<FontWeight> fontWeights = new List<FontWeight>();
                List<TextDecoration> textDecorations = new List<TextDecoration>();

                float fontSize = 16;
                Color color = Color.Black;

                List<Rule> inheritedStylesCopy = new List<Rule>();
                inheritedStylesCopy.AddRange(inheritedStyles);

                // Set styles for current element used by its parents.
                // For example if a div is in b and the b is in i,
                // the div's text will be bold and italic.

                foreach (Rule rule in inheritedStyles) {
                    if (rule.Property == "font-weight") {
                        if (rule.Value == "bold") fontWeights.Add(FontWeight.Bold);
                        else if (rule.Value == "normal") fontWeights.Add(FontWeight.Normal);
                    } else if (rule.Property == "font-style") {
                        if (rule.Value == "italic") fontStyles.Add(FontStyle.Italic);
                        else if (rule.Value == "normal") fontStyles.Add(FontStyle.Normal);
                    } else if (rule.Property == "font-family") {
                        fontFamily = rule.Value;
                    } else if (rule.Property == "color") {
                        color = ColorTranslator.FromHtml(rule.Value);
                        element.Style.Color = color;
                    } else if (rule.Property == "text-decoration") {
                        if (rule.Value == "underline") {
                            textDecorations.Add(TextDecoration.Underline);
                        }
                    } else if (rule.Property == "font-size") {
                        CSSValue parsedValue = CSSUnits.ParseValue(rule, element);

                        fontSize = parsedValue.Value;

                        inheritedStylesCopy.Remove(rule);
                    }
                }

                // Set styles for current element.
                if (element.Type == DOMElementType.Normal) {
                    foreach (RuleSet ruleSet in ruleSets) {
                        // Check if the selector in the rule set is matching current element's selector.
                        if (element.HasSelector(ruleSet.Selector)) {
                            element.RuleSet = ruleSet;

                            foreach (Rule rule in ruleSet.Rules) {
                                if (rule.Property == "font-weight") {
                                    if (rule.Value == "bold") fontWeights.Add(FontWeight.Bold);
                                    else if (rule.Value == "normal") fontWeights.Add(FontWeight.Normal);

                                    newInheritedStyles.Add(rule);
                                } else if (rule.Property == "font-style") {
                                    if (rule.Value == "italic") fontStyles.Add(FontStyle.Italic);
                                    else if (rule.Value == "normal") fontStyles.Add(FontStyle.Normal);

                                    newInheritedStyles.Add(rule);
                                } else if (rule.Property == "font-size") {
                                    CSSValue parsedValue = CSSUnits.ParseValue(rule, element);

                                    fontSize = parsedValue.Value;

                                    newInheritedStyles.Add(rule);
                                } else if (rule.Property == "font-family") {
                                    fontFamily = rule.Value;

                                    newInheritedStyles.Add(rule);
                                } else if (rule.Property == "color") {
                                    color = ColorTranslator.FromHtml(rule.Value);
                                    element.Style.Color = color;

                                    newInheritedStyles.Add(rule);
                                } else if (rule.Property == "text-decoration") {
                                    if (rule.Value == "underline") textDecorations.Add(TextDecoration.Underline);

                                    newInheritedStyles.Add(rule);
                                }
                            }
                        }
                    }
                }

                // Apply all the font styles added before.
                Font font = new Font(new FontFamily(fontFamily), fontSize, System.Drawing.FontStyle.Regular, GraphicsUnit.Pixel);

                foreach (FontStyle fontStyle in fontStyles) {
                    if (fontStyle == FontStyle.Italic) font = new Font(font, font.Style ^ System.Drawing.FontStyle.Italic);
                }

                foreach (FontWeight fontWeight in fontWeights) {
                    if (fontWeight == FontWeight.Bold) font = new Font(font, font.Style ^ System.Drawing.FontStyle.Bold);
                    else if (fontWeight == FontWeight.Normal) font = new Font(font, font.Style ^ System.Drawing.FontStyle.Regular);
                }

                foreach (TextDecoration textDecoration in textDecorations) {
                    if (textDecoration == TextDecoration.Underline) font = new Font(font, font.Style ^ System.Drawing.FontStyle.Underline);
                }

                element.Style.Font = font;

                // Pass used styles by the current element and its parents, to its children.
                foreach (Rule rule in inheritedStylesCopy) {
                    newInheritedStyles.Add(rule);
                }

                if (element.Children.Count > 0) SetFonts(element.Children, ruleSets, newInheritedStyles);
            }
        }

        private static void SetStyles (List<DOMElement> elements, List<RuleSet> ruleSets, List<Rule> inheritedStyles = null) {
            if (inheritedStyles == null) inheritedStyles = new List<Rule>();

            foreach (DOMElement element in elements) {
                List<Rule> newInheritedStyles = new List<Rule>();
                List<Rule> inheritedStylesCopy = new List<Rule>();
                inheritedStylesCopy.AddRange(inheritedStyles);

                // Set styles for current element used by its parents.

                foreach (Rule rule in inheritedStyles) {
                    if (rule.Property == "padding-top") {
                        float paddingTop = float.Parse(rule.Value.Split(new string[] { "px" }, StringSplitOptions.None)[0]);
                        element.ComputedStyle.Padding.Top = paddingTop;

                        inheritedStylesCopy.Remove(rule);
                    } else if (rule.Property == "padding-left") {
                        float paddingLeft = float.Parse(rule.Value.Split(new string[] { "px" }, StringSplitOptions.None)[0]);
                        element.ComputedStyle.Padding.Left = paddingLeft;

                        inheritedStylesCopy.Remove(rule);
                    }
                }

                // Set styles for current element.
                if (element.Type == DOMElementType.Normal) {
                    foreach (RuleSet ruleSet in ruleSets) {
                        // Check if the selector in the rule set is matching current element's selector.
                        if (element.HasSelector(ruleSet.Selector)) {
                            element.RuleSet = ruleSet;

                            foreach (Rule rule in ruleSet.Rules) {
                                if (rule.Property == "padding-top") {
                                    element.Style.Padding.Top = CSSUnits.ParseValue(rule, element).Value;

                                    newInheritedStyles.Add(rule);
                                } else if (rule.Property == "padding-left") {
                                    element.Style.Padding.Left = CSSUnits.ParseValue(rule, element).Value;

                                    newInheritedStyles.Add(rule);
                                } else if (rule.Property == "padding-right") {
                                    element.Style.Padding.Right = CSSUnits.ParseValue(rule, element).Value;
                                } else if (rule.Property == "padding-bottom") {
                                    element.Style.Padding.Bottom = CSSUnits.ParseValue(rule, element).Value;
                                } else if (rule.Property == "margin-top") {
                                    element.Style.Margin.Top = CSSUnits.ParseValue(rule, element).Value;
                                } else if (rule.Property == "margin-bottom") {
                                    element.Style.Margin.Bottom = CSSUnits.ParseValue(rule, element).Value;
                                } else if (rule.Property == "margin-left") {
                                    element.Style.Margin.Left = CSSUnits.ParseValue(rule, element).Value;
                                } else if (rule.Property == "margin-right") {
                                    element.Style.Margin.Right = CSSUnits.ParseValue(rule, element).Value;
                                } else if (rule.Property == "height") {
                                    element.Style.Size.Height = CSSUnits.ParseValue(rule, element).Value;
                                } else if (rule.Property == "width") {
                                    element.Style.Size.Width = CSSUnits.ParseValue(rule, element).Value;
                                } else if (rule.Property == "background-color") {
                                    element.Style.BackgroundColor = ColorTranslator.FromHtml(rule.Value);
                                } else if (rule.Property == "border") {
                                    float borderWidth = float.Parse(rule.Value.Split(' ')[0].Split(new string[] { "px" }, StringSplitOptions.None)[0]);
                                    string borderType = rule.Value.Split(' ')[1];
                                    Color borderColor = ColorTranslator.FromHtml(rule.Value.Split(' ')[2]);

                                    element.Style.Border = new Border(borderWidth, BorderType.Solid, borderColor);
                                } else if (rule.Property == "display") {
                                    if (rule.Value == "none") element.Style.Display = Display.None;
                                    else if (rule.Value == "block") element.Style.Display = Display.Block;
                                    else if (rule.Value == "initial") element.Style.Display = Display.Initial;
                                    else if (rule.Value == "inherit") element.Style.Display = Display.Inherit;
                                    else if (rule.Value == "flex") element.Style.Display = Display.Flex;
                                    else if (rule.Value == "inline-flex") element.Style.Display = Display.InlineFlex;
                                    else if (rule.Value == "inline-table") element.Style.Display = Display.InlineTable;
                                    else if (rule.Value == "inline") element.Style.Display = Display.Inline;
                                    else if (rule.Value == "inline-block") element.Style.Display = Display.InlineBlock;
                                    else if (rule.Value == "list-item") element.Style.Display = Display.ListItem;
                                    else if (rule.Value == "run-in") element.Style.Display = Display.RunIn;
                                    else if (rule.Value == "table") element.Style.Display = Display.Table;
                                    else if (rule.Value == "table-cell") element.Style.Display = Display.TableCell;
                                    else if (rule.Value == "table-row") element.Style.Display = Display.TableRow;
                                    else if (rule.Value == "table-caption") element.Style.Display = Display.TableCaption;
                                    else if (rule.Value == "table-column-group") element.Style.Display = Display.TableColumnGroup;
                                    else if (rule.Value == "table-header-group") element.Style.Display = Display.TableHeaderGroup;
                                    else if (rule.Value == "table-footer-group") element.Style.Display = Display.TableFooterGroup;
                                    else if (rule.Value == "table-row-group") element.Style.Display = Display.TableRowGroup;
                                    else if (rule.Value == "table-column") element.Style.Display = Display.TableColumn;
                                }
                            }
                        }
                    }
                }

                // Pass used styles by the current element and its parents, to its children.
                foreach (Rule rule in inheritedStylesCopy) {
                    newInheritedStyles.Add(rule);
                }

                if (element.Children.Count > 0) SetStyles(element.Children, ruleSets, newInheritedStyles);
            }
        }

        private static void SetSizes (List<DOMElement> elements) {
            foreach (DOMElement element in elements) {
                if (element.Children.Count > 0) SetSizes(element.Children);
                else element.SetSizes(bmp);
            }
        }

        private static void SetPercentSizes (List<DOMElement> elements) {
            foreach (DOMElement element in elements) {
                element.SetPercentSizes();
                if (element.Children.Count > 0) SetPercentSizes(element.Children);
            }
        }

        private static void SetPositions (List<DOMElement> elements) {
            foreach (DOMElement element in elements) {
                if (elements.IndexOf(element) > 0) {
                    DOMElement prevElement = elements[elements.IndexOf(element) - 1];

                    if (element.Style.Display == Display.InlineBlock || element.Style.Display == Display.TableCell) {
                        element.ComputedStyle.Position.Y = prevElement.ComputedStyle.Position.Y + element.ComputedStyle.Padding.Top;
                        element.ComputedStyle.Position.X = 0;

                        if (prevElement.Style.Display == element.Style.Display) {
                            element.ComputedStyle.Position.X += prevElement.ComputedStyle.Position.X + prevElement.ComputedStyle.Size.Width;
                        } else {
                            element.ComputedStyle.Position.Y += prevElement.ComputedStyle.Size.Height + prevElement.Style.Margin.Bottom + element.Style.Margin.Top;
                        }

                        element.ComputedStyle.Position.X += (prevElement.Style.Margin.Right >= element.Style.Margin.Left)
                            ? prevElement.Style.Margin.Right
                            : element.Style.Margin.Left;

                    } else if (element.Style.Display == Display.Block || element.Style.Display == Display.TableRow || element.Style.Display == Display.Table) {
                        element.ComputedStyle.Position.Y = prevElement.ComputedStyle.Position.Y + prevElement.ComputedStyle.Size.Height;
                        if (element.Parent != null) element.ComputedStyle.Position.X = element.Parent.ComputedStyle.Position.X + element.ComputedStyle.Padding.Left;

                        element.ComputedStyle.Position.Y += (prevElement.Style.Margin.Bottom >= element.Style.Margin.Top)
                            ? prevElement.Style.Margin.Bottom
                            : element.Style.Margin.Top;

                        element.ComputedStyle.Position.X += (prevElement.Style.Margin.Right >= element.Style.Margin.Left)
                            ? prevElement.Style.Margin.Right
                            : element.Style.Margin.Left;

                    }

                    if (prevElement.Style.Display == Display.None) {
                        element.ComputedStyle.Position.X = prevElement.ComputedStyle.Position.X;
                        element.ComputedStyle.Position.Y = prevElement.ComputedStyle.Position.Y;
                    }
                } else {
                    element.ComputedStyle.Position.Y = element.Style.Margin.Top;
                    element.ComputedStyle.Position.X = element.Style.Margin.Left;

                    if (element.Parent != null) {
                        element.ComputedStyle.Position.Y += element.Parent.ComputedStyle.Position.Y;
                        element.ComputedStyle.Position.X += element.Parent.ComputedStyle.Position.X;
                    }

                    element.ComputedStyle.Position.Y += element.ComputedStyle.Padding.Top;
                    element.ComputedStyle.Position.X += element.ComputedStyle.Padding.Left;
                }

                if (element.Children.Count > 0) SetPositions(element.Children);
            }
        }
    }
}
