using System;
using System.Collections.Generic;
using System.Drawing;

namespace LayoutEngine {
    public class DOMElement {
        public string Content = "";
        public int Level;
        public List<DOMElement> Children = new List<DOMElement>();
        public DOMElement Parent;
        public HTMLTag Tag;
        public DOMElementType Type;
        public Style Style = new Style();
        public ComputedStyle ComputedStyle = new ComputedStyle();
        public RuleSet RuleSet = new RuleSet();

        public void SetSizes (Bitmap bmp, Size childrenSizes = new Size()) {
            Size newChildrenSizes = new Size(0, 0);

            if (this.Type == DOMElementType.Normal) {
                this.ComputedStyle.Size.Width = (this.Style.Size.Width != -1) ? this.Style.Size.Width : childrenSizes.Width;
                this.ComputedStyle.Size.Height = (this.Style.Size.Height != -1) ? this.Style.Size.Height : childrenSizes.Height;
            } else if (this.Type == DOMElementType.Text) {
                Font font = this.Style.Font;

                using (Graphics graphics = Graphics.FromImage(bmp)) {
                    float textWidth = graphics.MeasureString(this.Content, font).Width;
                    float textHeight = graphics.MeasureString(this.Content, font).Height;

                    this.ComputedStyle.Size.Height = (this.Style.Size.Height == -1) ? textHeight - 2 : this.Style.Size.Height;
                    this.ComputedStyle.Size.Width = (this.Style.Size.Width == -1) ? textWidth : this.Style.Size.Width;
                }
            }

            this.SetSizesWithPaddings(childrenSizes);

            if (this.Parent != null) newChildrenSizes = this.GetChildrenSizes();

            if (this.Style.Display == Display.Table) this.SetTableSizes();

            if (this.Parent != null) this.Parent.SetSizes(bmp, newChildrenSizes);
        }

        public void SetSizesWithPaddings (Size childrenSizes = new Size()) {
            float height = this.ComputedStyle.Size.Height;
            float width = this.ComputedStyle.Size.Width;

            Padding padding = this.Style.Padding;
            Size size = this.ComputedStyle.Size;

            if (padding.Top + childrenSizes.Height >= size.Height) height += padding.Top;
            if (padding.Left + childrenSizes.Width >= size.Width) width += padding.Left;
            if (padding.Left + padding.Right + childrenSizes.Width >= size.Width) width += padding.Right;
            if (padding.Top + padding.Bottom + childrenSizes.Height >= size.Height) height += padding.Bottom;

            this.ComputedStyle.Size.Width = width;
            this.ComputedStyle.Size.Height = height;
        }

        public void SetTableSizes () {
            foreach (DOMElement child in this.Children) {
                // Find table row.
                if (child.Style.Display == Display.TableRow) {
                    child.ComputedStyle.Size.Width = 0;

                    foreach (DOMElement child1 in this.Children) {
                        // Find second table row to compare.
                        if (child1.Style.Display == Display.TableRow && child1 != child) {
                            // Get table cells in first and second table rows.
                            for (int i = 0; i < child1.Children.Count; i++) {
                                if (!(i > child.Children.Count - 1) && !(i > child1.Children.Count - 1)) {
                                    // Get the biggest width in table cells.
                                    if (child1.Children[i].ComputedStyle.Size.Width > child.Children[i].ComputedStyle.Size.Width) {
                                        // Set the biggest width in table cell.
                                        child.Children[i].ComputedStyle.Size.Width = child1.Children[i].ComputedStyle.Size.Width;
                                    }
                                }
                            }
                        }
                    }

                    // Increase width of the first table cell by table cells width.
                    foreach (DOMElement tableCell in child.Children) {
                        child.ComputedStyle.Size.Width += tableCell.ComputedStyle.Size.Width;
                    }

                    float maxWidth = 0;

                    // Get the biggest width of table rows.
                    foreach (DOMElement child1 in this.Children) {
                        // Set the biggest width for other table rows.
                        if (child1.ComputedStyle.Size.Width > maxWidth) maxWidth = child1.ComputedStyle.Size.Width;
                    }

                    child.ComputedStyle.Size.Width = maxWidth;
                }
            }
        }

        public Size GetChildrenSizes () {
            Size childrenSize = new Size(0, 0);

            foreach (DOMElement child in this.Parent.Children) {
                Style currStyle = child.Style;
                ComputedStyle currCompStyle = child.ComputedStyle;

                if (currStyle.Display == Display.Block || currStyle.Display == Display.Table || currStyle.Display == Display.TableRow) {
                    // Increase height by child's height.
                    childrenSize.Height += currCompStyle.Size.Height;
                    // Set the biggest width in all children.
                    if (currCompStyle.Size.Width > childrenSize.Width) childrenSize.Width = currCompStyle.Size.Width;
                } else if (currStyle.Display == Display.InlineBlock || currStyle.Display == Display.TableCell) {
                    // If the child isn't first.
                    if (this.Parent.Children.IndexOf(child) > 0) {
                        Style prevStyle = this.Parent.Children[this.Parent.Children.IndexOf(child) - 1].Style;
                        // If the display is the same as previous this's display,
                        // the this can be rendered inline.
                        if (prevStyle.Display == currStyle.Display) {
                            // Set the biggest height in all children.
                            if (currCompStyle.Size.Height > childrenSize.Height) childrenSize.Height = currCompStyle.Size.Height;
                        } else {
                            // Increase height by child's height.
                            childrenSize.Height += currCompStyle.Size.Height;
                        }
                    }
                    // If it is.
                    else {
                        // Increase height by child's height.
                        childrenSize.Height += currCompStyle.Size.Height;
                    }
                    // Increase width by child's height.
                    childrenSize.Width += currCompStyle.Size.Width;
                }
            }

            return childrenSize;
        }

        public void SetPercentSizes () {
            if (this.RuleSet.Rules != null) {
                foreach (Rule rule in this.RuleSet.Rules) {
                    if (rule.ComputedValue != null) {
                        float value = -1f;

                        if (rule.ComputedValue.Unit == Unit.Percent) {
                            value = CSSUnitsConverter.PercentToPx(rule, this);
                        } else if (rule.ComputedValue.Unit == Unit.CalcFunc) {
                            value = CSSCalcFunction.Calculate(rule, this);
                        }

                        if (value != -1f) {
                            if (rule.Property == "width") {
                                this.ComputedStyle.Size.Width = value;
                            } else if (rule.Property == "height") {
                                this.ComputedStyle.Size.Height = value;
                            }
                        }
                    }
                }
            }
        }

        public bool HasSelector (string selector) {
            string tag = this.Tag.Name;
            List<string> classes = new List<string>();
            string id = "";

            foreach (HTMLAttribute attr in this.Tag.Attributes) {
                if (attr.Property == "class") {
                    foreach (string className in attr.Value.ToLower().Split(' ')) {
                        classes.Add(className);
                    }
                } else if (attr.Property == "id") {
                    id = attr.Value;
                }
            }

            bool canUseStyle = false;
            string tagSelector = selector;
            List<string> classesSelector = new List<string>();
            string idSelector = "";

            if (tagSelector == tag) canUseStyle = true;

            tagSelector = "";

            int startingIndex = 0;

            string temp = "";

            for (int i = 0; i < selector.Length; i++) {
                if (selector[i] == '.') startingIndex = 1;
                if (selector[i] == '.' || selector[i] == '#') break;

                temp += selector[i];
            }

            if (tagSelector != "") tagSelector = temp;

            if (tagSelector == tag) {
                canUseStyle = true;
            } else if (tagSelector == "") {
                tagSelector = tag;
            }

            if (tagSelector == tag && !canUseStyle) {
                if (selector.Contains("#")) {
                    idSelector = selector.Split('#')[1];
                    if (idSelector == id) canUseStyle = true;
                } else if (selector.Contains(".")) {
                    for (int i = startingIndex; i < selector.Split('.').Length; i++) {
                        foreach (string className in classes) {
                            if (selector.Split('.')[i] == className) {
                                canUseStyle = true;
                                break;
                            }
                        }
                    }
                }
            }

            return canUseStyle;
        }
    }
}
