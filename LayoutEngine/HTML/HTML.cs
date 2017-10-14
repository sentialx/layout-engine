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

            HTMLDocument htmlDocument = new HTMLDocument
            {
                Children = domTree
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
                htmlTag.attributes = ParseAttributes(tagCode);
            }

            element.Tag = htmlTag;

            return element;
        }

        private static List<HTMLTagAttribute> ParseAttributes(string tagCode)
        {
            List<HTMLTagAttribute> attributesList = new List<HTMLTagAttribute>();
            // Get spaces indexes.
            List<int> spaceIndexes = Utils.GetIndexes(tagCode, " ");

            int lastAttributeWithValueEndIndex = -1;

            for (int i = 0; i < spaceIndexes.Count; i++)
            {
                int spaceIndex = spaceIndexes[i];

                // If next sign isn't space.
                if (tagCode[spaceIndex + 1].ToString() != " ")
                {
                    bool isEmptyAttribute = true;
                    bool addAttribute = true;

                    // Initialize an attribute.
                    HTMLTagAttribute attribute = new HTMLTagAttribute();

                    for (int n = spaceIndex + 1; n < tagCode.Length; n++)
                    {
                        string sign = tagCode[n].ToString();

                        if (sign == " " || sign == "=" || sign == ">")
                        {
                            
                            // Cut tag code to get the name.
                            string name = tagCode.Substring(spaceIndex + 1, n - spaceIndex - 1);

                            if (name.Length > 0)
                            {
                                isEmptyAttribute = false;
                                attribute.name = name;
                            }

                            break;
                        }
                    }

                    // Potential equal index.
                    int equalIndex = spaceIndex + 1 + attribute.name.Length;
                    attribute.startIndex = equalIndex + 1;

                    if (spaceIndex < lastAttributeWithValueEndIndex)
                    {
                        addAttribute = false;
                        isEmptyAttribute = true;
                    }

                    if (!isEmptyAttribute)
                    {
                        if (equalIndex < tagCode.Length)
                        {
                            string signAtPotentialEqualIndex = tagCode[equalIndex].ToString();

                            if (equalIndex + 1 < tagCode.Length) {
                                string nextSign = tagCode[equalIndex + 1].ToString();
                                // Check if attribute has a value.
                                if (signAtPotentialEqualIndex == "=" && nextSign != " " && nextSign != ">")
                                {
                                    // Attribute value is in quotation marks. 
                                    bool isQuotationMark = (nextSign == "\"");

                                    int valueStartIndex = equalIndex + 1;
                                    if (isQuotationMark) valueStartIndex++;

                                    for (int v = valueStartIndex; v < tagCode.Length; v++)
                                    {
                                        string sign = tagCode[v].ToString();

                                        // Check if the value is closed.
                                        bool condition = (sign == ">" || sign == @"""") || !isQuotationMark && sign == " ";

                                        if (condition)
                                        {
                                            int startIndex = equalIndex + 1;
                                            int endIndex = v - equalIndex - 1;

                                            if (isQuotationMark)
                                            {
                                                startIndex++;
                                                endIndex--;
                                            }

                                            // Cut tag code to get attribute the value.
                                            attribute.endIndex = endIndex;
                                            attribute.value = tagCode.Substring(startIndex, endIndex);

                                            lastAttributeWithValueEndIndex = equalIndex + v - equalIndex + 1;

                                            break;
                                        }
                                    }
                                 }
                            }                        
                        }

                        // Add attribute to attributes list.
                        if (addAttribute) attributesList.Add(attribute);
                    }
                }
            }

            return attributesList;
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
    }
}
