using System;
using System.Collections.Generic;

namespace LayoutEngine
{
    public class HTML
    {
        public static string Minify(string[] html)
        {
            string htmlMin = "";

            for (int i = 0; i < html.Length; i++)
            {
                htmlMin += html[i].Trim();
            }

            return htmlMin;
        }

        public static List<string> Beautify(string html)
        {
            return Beautify(Parse(html).Children);
        }

        private static List<string> Beautify(List<DOMElement> elements)
        {
            List<string> lines = new List<string>();

            foreach (DOMElement element in elements)
            {
                string line = "";

                for (int y = 0; y < element.Level; y++)
                {
                    line += "  ";
                }

                if (element.Type == DOMElementType.Text)
                {
                    lines.Add(line + element.Content);
                }
                else if (element.Type == DOMElementType.Normal)
                {
                    lines.Add(line + element.Tag.Code);
                }

                if (element.Type == DOMElementType.Normal && element.Tag.Type != TagType.SelfClosing)
                {
                    if (element.Children.Count > 0)
                    {
                        List<string> lines2 = Beautify(element.Children);
                        foreach (string line1 in lines2)
                        {
                            lines.Add(line1);
                        }
                    }
                    else
                    {
                        lines.Add(line + "  " + element.Content);
                    }

                    lines.Add(line + "</" + element.Tag.Name + ">");
                }
            }
            return lines;
        }

        public static HTMLDocument Parse(string html)
        {
            // Remove new line characters from html string.
            html = html.Replace(System.Environment.NewLine, "").Trim();

            // Get all elements (tags, text etc.)
            List<Element> elements = GetElements(html);

            // Set levels for elements.
            SetLevels(elements);

            // Get DOM tree.
            List<DOMElement> domTree = GetDOMTree(elements);

            List<Meta> metaTags = GetMetaTags(domTree);

            HTMLDocument htmlDocument = new HTMLDocument
            {
                Children = domTree,
                MetaTags = metaTags
            };

            return htmlDocument;
        }

        private static Element ParseTag(int startingIndex, string html)
        {
            // Initialize an Element.
            Element element = new Element()
            {
                Index = startingIndex,
                Type = ElementType.Tag
            };

            string tagCode = "";
            string tagName = "";

            // Get tag code.
            // For example it gets <div class="menu">.
            for (int y = startingIndex; y < html.Length; y++)
            {
                tagCode += html[y];
                if (html[y] == '>') break;
            }

            // Determine the tag's type.
            TagType tagType = TagType.Opening;
            if (tagCode.StartsWith("</")) tagType = TagType.Closing;
            else if (tagCode.EndsWith("/>")) tagType = TagType.SelfClosing;

            // Get starting index of tag's name.
            int index = 1;
            if (tagType == TagType.Closing) index = 2;

            // Get tag name.
            // For example if tag's code is <div class="menu"> then its tag name is div.
            for (int y = index; y < tagCode.Length; y++)
            {
                if (tagCode[y] == ' ' || tagCode[y] == '/' || tagCode[y] == '>') break;
                tagName += tagCode[y];
            }

            // Initialize a HTMLTag.
            HTMLTag htmlTag = new HTMLTag
            {
                Code = tagCode.Trim(),
                Name = tagName.Trim(),
                Type = tagType
            };

            if (tagType == TagType.Opening)
            {
                htmlTag.Attributes = ParseAttributes(tagCode);
            }

            element.Tag = htmlTag;

            return element;
        }

        private static Element ParseText(int startingIndex, string html)
        {
            // Initialize an Element.
            Element element = new Element()
            {
                Index = startingIndex,
                Type = ElementType.Text
            };

            string text = "";

            // Get the text.
            for (int y = startingIndex; y < html.Length; y++)
            {
                if (html[y] == '<')
                {
                    if (html[y + 1] != ' ' && html[y + 2] != ' ' && html[y + 1] != '<')
                    {
                        bool isValidTag = false;
                        for (int z = y + 1; z < html.Length; z++)
                        {
                            if (html[z] == '<')
                            {
                                isValidTag = false;
                                break;
                            }
                            else if (html[z] == '>')
                            {
                                isValidTag = true;
                                break;
                            }
                        }
                        // Break if char at position y starts a valid HTML tag.
                        if (isValidTag)
                        {
                            break;
                        }
                    }
                }
                text += html[y];
            }

            element.Content = text.Trim();

            return element;
        }

        private static List<Element> GetElements(string html)
        {
            List<Element> elements = new List<Element>();

            bool isInTag = false;
            bool isText = false;

            for (int x = 0; x < html.Length; x++)
            {
                // The character at x probably starts a valid HTML tag.
                if (html[x] == '<')
                {
                    // It's probably a valid tag, but we still don't know.
                    if (html[x + 1] != ' ')
                    {
                        // Check if the tag ends with '>'.
                        bool isValidTag = false;
                        for (int y = x + 1; y < html.Length; y++)
                        {
                            if (html[y] == '<')
                            {
                                isValidTag = false;
                                break;
                            }
                            else if (html[y] == '>')
                            {
                                isValidTag = true;
                                break;
                            }
                        }
                        // Yes, the tag is valid!
                        if (isValidTag)
                        {
                            // Let's parse the tag.
                            Element element = ParseTag(x, html);

                            elements.Add(element);

                            // Character at x is inside the tag, and it's not a regular text.
                            isInTag = true;
                            isText = false;
                        }
                    }
                }
                else if (!isInTag && !isText) // Character at x isn't in tag and isn't a regular text.
                {
                    // Let's parse the regular text.
                    Element element = ParseText(x, html);

                    elements.Add(element);

                    // Character at x is in a regular text.
                    isText = true;
                }
                else if (html[x] == '>' && isInTag)
                {
                    // Character at x isn't inside a tag or a regular text.
                    isInTag = false;
                    isText = false;
                }
            }
            return elements;
        }

        private static void SetLevels(List<Element> elements)
        {
            int currentLevel = 0;

            for (int x = 0; x < elements.Count; x++)
            {
                // If element is a text, and previous element is a tag, and previous element is an opening tag, and the element is not first.
                if (elements[x].Type == ElementType.Text)
                {
                    if (x - 1 >= 0 && elements[x - 1].Type == ElementType.Tag && elements[x - 1].Tag.Type == TagType.Opening && x != 0)
                    {
                        currentLevel++;
                    }

                    // Set level for the text element.
                    elements[x].Level = currentLevel;
                }
                // If element is a tag
                else if (elements[x].Type == ElementType.Tag)
                {
                    // If the tag is a closing tag
                    if (elements[x].Tag.Type == TagType.Closing)
                    {
                        // If previous element is a tag, and previous element is a closing tag
                        if (elements[x - 1].Type == ElementType.Tag && elements[x - 1].Tag.Type == TagType.Closing)
                        {
                            currentLevel--;
                        }
                        // If previous element is a text
                        else if (elements[x - 1].Type == ElementType.Text)
                        {
                            currentLevel--;
                        }
                    }
                    // If the element is an opening tag
                    else if (elements[x].Tag.Type == TagType.Opening)
                    {
                        // If the element is not first, and previous element is a tag, and previous element is an opening tag.
                        if (x != 0 && elements[x - 1].Type == ElementType.Tag && elements[x - 1].Tag.Type == TagType.Opening)
                        {
                            currentLevel++;
                        }
                    }

                    // Set level for the tag element.
                    elements[x].Level = currentLevel;
                }
            }
        }

        private static List<DOMElement> GetDOMTree(List<Element> elements)
        {
            List<DOMElement> domElements = new List<DOMElement>();
            List<DOMElement> domTree = new List<DOMElement>();

            for (int x = 0; x < elements.Count; x++)
            {
                // Initialize an DOMElement.
                DOMElement domElement = new DOMElement()
                {
                    Level = elements[x].Level
                };

                if (elements[x].Type == ElementType.Text)
                {
                    domElement.Content = elements[x].Content;
                    domElement.Type = DOMElementType.Text;
                }
                else if (elements[x].Type == ElementType.Tag)
                {
                    domElement.Type = DOMElementType.Normal;
                    domElement.Tag = elements[x].Tag;
                }

                bool canBeProcessed = false;

                if (elements[x].Type == ElementType.Text)
                {
                    canBeProcessed = true;
                }
                else if (elements[x].Type == ElementType.Tag)
                {
                    canBeProcessed = elements[x].Tag.Type == TagType.Opening || elements[x].Tag.Type == TagType.SelfClosing;
                }

                if (elements[x].Level != 0)
                {
                    if (canBeProcessed)
                    {
                        DOMElement parent = null;

                        // Get parent of current DOMElement.
                        for (int y = domElements.Count - 1; y >= 0; y--)
                        {
                            if (domElements[y].Level == domElement.Level - 1)
                            {
                                parent = domElements[y];
                                break;
                            }
                        }

                        if (parent != null)
                        {
                            // Set parent of current DOMElement.
                            domElement.Parent = parent;
                            // Add current DOMElement to parent's children list.
                            parent.Children.Add(domElement);
                        }
                    }
                }
                else
                {
                    if (canBeProcessed)
                    {
                        // Add DOMElements to domTree that have only level 0.
                        domTree.Add(domElement);
                    }
                }

                domElements.Add(domElement);
            }

            return domTree;
        }

        /// <summary>
        /// Parses tag code into attributes
        /// </summary>
        private static List<HTMLAttribute> ParseAttributes(string tagCode)
        {
            List<HTMLAttribute> attributesList = new List<HTMLAttribute>();
            List<int> spaceIndexes = Utils.GetIndexes(tagCode, " "); // Get indexes of all spaces

            foreach (int spaceIndex in spaceIndexes)
            {
                int index = spaceIndex + 1;
                char nextSign = tagCode[index];
                
                // Loop is used for parsing attributes next to each other
                // For example
                // message="example"user="x"readonly
                while (true)
                {
                    // Check if there is attribute not separated with space
                    if (nextSign != ' ' && nextSign != '>')
                    {
                        HTMLAttribute attribute = ParseAttribute(tagCode, index);

                        // Check if during attribute parsing, occurred an error
                        if (attribute == null) return new List<HTMLAttribute>();

                        attributesList.Add(attribute);

                        if (attribute.HasValue) index = attribute.ValueEndIndex + 1;
                        else break;
                    } else
                    {
                        break;
                    }
                }
            }

            // Parsed attributes usually aren't correct, so incorrect attributes must be removed
            return FixHTMLAttributes(attributesList);
        }

        /// <summary>
        /// Fixes attributes list
        /// </summary>
        private static List<HTMLAttribute> FixHTMLAttributes (List<HTMLAttribute> list)
        {
            List<HTMLAttribute> fixedList = new List<HTMLAttribute>();

            // Add first attribute
            if (list.Count > 0) fixedList.Add(list[0]);

            HTMLAttribute lastAttributeWithValue = new HTMLAttribute();

            for (int i = 0; i < list.Count; i++)
            {
                HTMLAttribute attribute = list[i];

                if (i > 0 && lastAttributeWithValue.PropertyStartIndex != 1 && attribute.Property.Length > 0)
                {
                    // If parsed attribute's start index is less than end index of value of latest attribute containing value is incorrect
                    // For example
                    // Actual start index = 18
                    // End index of value of latest attribute = 21
                    // 18 < 21 so it's incorrect
                    if (attribute.PropertyStartIndex > lastAttributeWithValue.ValueEndIndex)
                    {
                        fixedList.Add(attribute);
                    }
                }

                if (attribute.HasValue) lastAttributeWithValue = attribute;
            }

            return fixedList;
        }

        /// <summary>
        /// Parses html tag code into attribute
        /// </summary>
        private static HTMLAttribute ParseAttribute(string code, int startIndex)
        {
            HTMLAttribute attribute = new HTMLAttribute();

            attribute.PropertyStartIndex = startIndex;

            // Check if attribute contains value and get property end index
            for (int i = startIndex; i < code.Length; i++)
            {
                char sign = code[i];

                // Attribute has value
                if (sign == '=')
                {
                    attribute.PropertyEndIndex = i;
                    attribute.HasValue = true;

                    break;
                } else if (sign == ' ' || sign == '>') // Attribute hasn't value
                {
                    attribute.PropertyEndIndex = i;

                    break;
                }
            }

            // Get property
            attribute.Property = code.Substring(attribute.PropertyStartIndex, attribute.PropertyEndIndex - attribute.PropertyStartIndex).ToLower();

            // Gets value
            if (attribute.HasValue)
            {
                char sign = code[attribute.PropertyEndIndex + 1];

                // If value doesn't starts with quotation mark
                if (sign != '"')
                {
                    attribute.ValueStartIndex = attribute.PropertyEndIndex + 1;

                    // Gets index closing value
                    for (int i = attribute.PropertyEndIndex + 1; i < code.Length; i++)
                    {
                        char _sign = code[i];

                        if (_sign == '"' || _sign == '>' || _sign == ' ')
                        {
                            attribute.ValueEndIndex = i;

                            break;
                        }
                    }
                }
                else // If value starts with quotation mark
                {
                    attribute.ValueStartIndex = attribute.PropertyEndIndex + 2;
                    // Get quotation mark closing value
                    attribute.ValueEndIndex = ParseAttributesGetQuotationMarkIndex(code, attribute.ValueStartIndex);
                }

                // Get value
                try
                {
                    attribute.Value = code.Substring(attribute.ValueStartIndex, attribute.ValueEndIndex - attribute.ValueStartIndex).ToLower();
                } catch
                {
                    Console.WriteLine("Attribute is incorrect (" + attribute.Property + ")");
                    return null;
                }
            }

            return attribute;
        }

        private static int ParseAttributesGetQuotationMarkIndex(string str, int startIndex)
        {
            for (int i = startIndex; i < str.Length; i++) if (str[i] == '"') return i;
            return -1;
        }

        private static List<Meta> GetMetaTags(List<DOMElement> elements)
        {
            List<Meta> metaTags = new List<Meta>();

            foreach (DOMElement element in elements)
            {
                if (element.Type == DOMElementType.Normal && element.Tag.Name.ToLower() == "meta")
                {
                    Meta tag = new Meta();

                    foreach (HTMLAttribute attribute in element.Tag.Attributes)
                    {
                        if (attribute.Property == "name")
                        {
                            tag.Property = MetaType.ViewportWidth;
                        }
                        else if (attribute.Property == "content")
                        {
                            tag.Value = attribute.Value;
                            tag.ComputedValue = ParseMetaViewPort(attribute.Value);
                        }
                    }

                    metaTags.Add(tag);

                    if (tag.Property == MetaType.ViewportWidth)
                    {
                        Meta height = new Meta();

                        height.Property = MetaType.ViewportHeight;
                        height.Value = tag.Value;
                        height.ComputedValue = ParseMetaViewPort(tag.Value, false);

                        metaTags.Add(height);
                    }
                }
            }

            return metaTags;
        }

        // TODO
        private static float ParseMetaViewPort(string content, bool width = true)
        {
            return width ? Program.deviceWidth : Program.deviceHeight;
        }
    }
}
