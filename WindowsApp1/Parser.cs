using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WindowsApp1
{
    static class Parser
    {

        /// <summary>
    /// This function parses the logic of the given token list and returns
    /// a parse tree.
    /// </summary>
    /// <param name="tokens">The token list as a linked list of string</param>
    /// <returns>A node that is the root of the parse tree representing the logic.</returns>
        public static Node Parse(LinkedList<string> tokens)
        {
            if (tokens.First.Value == "SOR")
            {
                tokens.RemoveFirst();
            }
            return ParseIns(tokens);
        }

        private static Node ParseIns(LinkedList<string> Tokens)
        {
            Node ans = null;
            if (Tokens.Count == 0)
            {
                return null;
            }
            string token = Tokens.First.Value;
            switch (token ?? "")
            {
                case "EQU":
                    {
                        ans = ParseEQU(Tokens);
                        break;
                    }
                case "GEQ":
                    {
                        ans = ParseGEQ(Tokens);
                        break;
                    }
                case "EOU":
                    {
                        ans = ParseEOU(Tokens);
                        break;
                    }
                case "NEQ":
                    {
                        ans = ParseNEQ(Tokens);
                        break;
                    }
                case "MOV":
                    {
                        ans = ParseMOV(Tokens);
                        break;
                    }
                case "BST":
                    {
                        ans = ParseBST(Tokens);
                        break;
                    }
                case "XIC":
                    {
                        ans = ParseXIC(Tokens);
                        break;
                    }
                case "XIO":
                    {
                        ans = ParseXIO(Tokens);
                        break;
                    }
                case "OR":
                    {
                        ans = ParseOR(Tokens);
                        break;
                    }
                case "JSR":
                    {
                        ans = ParseJSR(Tokens);
                        break;
                    }
                case "SWP":
                    {
                        ans = ParseSWP(Tokens);
                        break;
                    }
                case "CPW":
                    {
                        ans = ParseCPW(Tokens);
                        break;
                    }

                default:
                    {
                        MessageBox.Show("Invalid instruction: " + token);
                        return null;
                    }
            }
            if (Tokens.First.Value != "EOR" && Tokens.First.Value != "NXB" && Tokens.First.Value != "BND")
            {
                ans.NextIns = ParseIns(Tokens);
            }
            return ans;
        }
        private static Node ParseCPW(LinkedList<string> Tokens)
        {
            var ans = new Node("CPW");
            Tokens.RemoveFirst();
            ans.Args.Add(Tokens.First.Value);
            Tokens.RemoveFirst();
            ans.Args.Add(Tokens.First.Value);
            Tokens.RemoveFirst();
            ans.Args.Add(Tokens.First.Value);
            Tokens.RemoveFirst();
            return ans;
        }
        private static Node ParseSWP(LinkedList<string> Tokens)
        {
            var ans = new Node("SWP");
            Tokens.RemoveFirst();
            ans.Args.Add(Tokens.First.Value);
            Tokens.RemoveFirst();
            ans.Args.Add(Tokens.First.Value);
            Tokens.RemoveFirst();
            return ans;
        }

        private static Node ParseOR(LinkedList<string> Tokens)
        {
            var ans = new Node("OR");
            Tokens.RemoveFirst();
            ans.Args.Add(Tokens.First.Value);
            Tokens.RemoveFirst();
            ans.Args.Add(Tokens.First.Value);
            Tokens.RemoveFirst();
            ans.Args.Add(Tokens.First.Value);
            Tokens.RemoveFirst();
            return ans;
        }
        private static Node ParseEQU(LinkedList<string> Tokens)
        {
            var ans = new Node("EQU");
            Tokens.RemoveFirst();
            ans.Args.Add(Tokens.First.Value);
            Tokens.RemoveFirst();
            ans.Args.Add(Tokens.First.Value);
            Tokens.RemoveFirst();
            return ans;
        }
        private static Node ParseGEQ(LinkedList<string> Tokens)
        {
            var ans = new Node("GEQ");
            Tokens.RemoveFirst();
            ans.Args.Add(Tokens.First.Value);
            Tokens.RemoveFirst();
            ans.Args.Add(Tokens.First.Value);
            Tokens.RemoveFirst();
            return ans;
        }
        private static Node ParseEOU(LinkedList<string> Tokens)
        {
            var ans = new Node("EOU");
            Tokens.RemoveFirst();
            ans.Args.Add(Tokens.First.Value);
            Tokens.RemoveFirst();
            ans.Args.Add(Tokens.First.Value);
            Tokens.RemoveFirst();
            return ans;
        }
        private static Node ParseNEQ(LinkedList<string> Tokens)
        {
            var ans = new Node("NEQ");
            Tokens.RemoveFirst();
            ans.Args.Add(Tokens.First.Value);
            Tokens.RemoveFirst();
            ans.Args.Add(Tokens.First.Value);
            Tokens.RemoveFirst();
            return ans;
        }
        private static Node ParseMOV(LinkedList<string> Tokens)
        {
            var ans = new Node("MOV");
            Tokens.RemoveFirst();
            ans.Args.Add(Tokens.First.Value);
            Tokens.RemoveFirst();
            ans.Args.Add(Tokens.First.Value);
            Tokens.RemoveFirst();
            return ans;
        }
        private static Node ParseXIC(LinkedList<string> Tokens)
        {
            var ans = new Node("XIC");
            Tokens.RemoveFirst();
            ans.Args.Add(Tokens.First.Value);
            Tokens.RemoveFirst();
            return ans;
        }
        private static Node ParseJSR(LinkedList<string> Tokens)
        {
            var ans = new Node("JSR");
            Tokens.RemoveFirst();
            ans.Args.Add(Tokens.First.Value);
            Tokens.RemoveFirst();
            return ans;
        }
        private static Node ParseXIO(LinkedList<string> Tokens)
        {
            var ans = new Node("XIO");
            Tokens.RemoveFirst();
            ans.Args.Add(Tokens.First.Value);
            Tokens.RemoveFirst();
            return ans;
        }
        private static Node ParseBST(LinkedList<string> Tokens)
        {
            Tokens.RemoveFirst();
            var ans = new Node("BST");
            while (Tokens.First.Value != "BND")
            {
                ans.Children.Add(ParseIns(Tokens));
                if (Tokens.Count == 0)
                {
                    MessageBox.Show("empty token!" + ans.ToString());
                }
                if (Tokens.First.Value == "NXB")
                {
                    Tokens.RemoveFirst();
                }
            }
            Tokens.RemoveFirst();
            return ans;
        }

    }


   
}