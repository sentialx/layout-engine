using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LayoutEngine {
    public class CSS {
        public static List<RuleSet> Parse (string css) {
            bool isInBrackets = false;
            bool readingSelector = false;

            // Remove new line characters from CSS string.
            css = css.Replace(System.Environment.NewLine, "");

            List<string> selectors = new List<string>();
            List<string> rules = new List<string>();
            List<RuleSet> ruleSets = new List<RuleSet>();

            // Get selectors and declarations (rules).
            for (int x = 0; x < css.Length; x++) {
                // Get declarations (rules).
                if (css[x] == '{') {
                    if (!isInBrackets) {
                        string rulesS = "";
                        for (int y = x; y < css.Length; y++) {
                            if (css[y] == '}') break;
                            if (css[y] != '{' && css[y] != '\n') {
                                rulesS += css[y];
                            }
                        }

                        if (rulesS != "") rules.Add(rulesS.Trim());

                        isInBrackets = true;
                    }

                    readingSelector = false;
                }

                // Get selectors.
                if (css[x] != '{' && css[x] != '}' && css[x] != ' ' && !isInBrackets) {
                    string selector = "";
                    if (!readingSelector) {
                        readingSelector = true;
                        for (int y = x; y < css.Length; y++) {
                            if (css[y] == '{' || css[y] == ' ') {
                                break;
                            }
                            selector += css[y];
                        }
                        selectors.Add(selector);
                    }
                }

                if (css[x] == '}') {
                    isInBrackets = false;
                    readingSelector = false;
                }
            }

            // Create rule sets.
            for (int x = 0; x < rules.Count; x++) {
                if (rules[x].Trim() != "") {
                    // Create rule set.
                    RuleSet ruleSet = new RuleSet();
                    ruleSet.Selector = selectors[x];

                    string[] rules2 = rules[x].Split(';');

                    List<Rule> realRules = new List<Rule>();

                    for (int y = 0; y < rules2.Length; y++) {
                        // If rule is valid.
                        if (rules2[y].Split(':').Length == 2) {
                            // Get property and value of rule.
                            string name = rules2[y].Split(':')[0].Trim();
                            string value = rules2[y].Split(':')[1].Trim();

                            // Create rule.
                            Rule rule = new Rule();
                            rule.Property = name;
                            rule.Value = value;

                            realRules.Add(rule);
                        }
                    }

                    ruleSet.Rules = realRules;

                    ruleSets.Add(ruleSet);
                }
            }
            return ruleSets;
        }
    }
}
