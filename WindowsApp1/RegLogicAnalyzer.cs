using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace WindowsApp1
{

    /// <summary>
/// RegLogicAnalyzer
/// This module analyzes the logic of the given parse tree and finds register mapping
/// within it. It is based on logic patterns that have been seen in the PLC programs.
/// </summary>
    public static class RegLogicAnalyzer
    {
        public static void FindRegMapping(Node root, List<Tuple<string, string>> results)
        {
            var cur = root;
            if (cur.Ins == "XIO")
            {
                cur = cur.NextIns;
                if (RegPattern0(cur, results))
                {
                    return;
                }
                if (RegPattern01(cur, results))
                {
                    return;
                }
                if (cur.Ins == "BST")
                {
                    foreach (var branch in cur.Children)
                    {
                        if (RegPattern1(branch, results))
                        {
                            continue;
                        }
                        if (RegPattern2(branch, results))
                        {
                            continue;
                        }
                        if (RegPattern3(branch, results))
                        {
                            continue;
                        }
                        if (RegPattern4(branch, results))
                        {
                            continue;
                        }
                        if (RegPattern5(branch, results))
                        {
                            continue;
                        }
                        MessageBox.Show("Register logic not found: " + branch.ToString());
                    }
                }
            }
        }

        // No Branch MOV case
        private static bool RegPattern01(Node root, List<Tuple<string, string>> results)
        {
            var cur = root;
            if (cur is not null && cur.Ins == "CPW")
            {
                string src = cur.Args(0);
                string des = WindowsApp1.AddressSolver.Tune(cur.Args(1));
                int offset = (int)Math.Round(cur.Args(2) - 1f);
                results.Add(new Tuple<string, string>(src, des));
                results.Add(new Tuple<string, string>(src, WindowsApp1.AddressSolver.AddrAdder(des, offset)));
                return true;
            }
            return false;
        }


        // No Branch CPW case
        private static bool RegPattern0(Node root, List<Tuple<string, string>> results)
        {
            var cur = root;
            if (cur is not null && cur.Ins == "MOV")
            {
                results.Add(new Tuple<string, string>(cur.Args(0), WindowsApp1.AddressSolver.Tune(cur.Args(1))));
                return true;
            }
            return false;
        }



        // MOV case
        private static bool RegPattern1(Node root, List<Tuple<string, string>> results)
        {
            var cur = root;
            if (cur is not null && cur.Ins == "MOV")
            {
                results.Add(new Tuple<string, string>(cur.Args(0), WindowsApp1.AddressSolver.Tune(cur.Args(1))));
                return true;
            }
            return false;
        }

        // EQU MOV case
        private static bool RegPattern2(Node root, List<Tuple<string, string>> results)
        {
            var cur = root;
            if (cur is not null && cur.Ins == "EQU")
            {
                cur = cur.NextIns;
                if (cur is not null && cur.Ins == "MOV")
                {
                    results.Add(new Tuple<string, string>(cur.Args(0), WindowsApp1.AddressSolver.Tune(cur.Args(1))));
                    return true;
                }
            }
            return false;
        }

        // EQU BST MOV MOV case
        private static bool RegPattern3(Node root, List<Tuple<string, string>> results)
        {
            var cur = root;
            if (cur is not null && cur.Ins == "EQU")
            {
                cur = cur.NextIns;
                if (cur is not null && cur.Ins == "BST" && cur.Children.Count == 2)
                {
                    Node branch1 = cur.Children(0);
                    Node branch2 = cur.Children(1);
                    if (RegPattern1(branch1, results))
                    {
                        if (branch2 is not null & branch1.Ins == "MOV")
                        {
                            return RegPattern1(branch2, results);
                        }
                    }
                }
            }
            return false;
        }

        // BST MOV EQU MOV case
        private static bool RegPattern4(Node root, List<Tuple<string, string>> results)
        {
            var cur = root;
            if (cur is not null && cur.Ins == "BST" && cur.Children.Count == 2)
            {
                Node branch1 = cur.Children(0);
                Node branch2 = cur.Children(1);
                if (branch1 is not null && branch1.Ins == "MOV")
                {
                    if (RegPattern1(branch1, results))
                    {
                        if (branch2 is not null && branch2.Ins == "EQU")
                        {
                            return RegPattern2(branch2, results);
                        }
                    }
                }
            }
            return false;
        }

        // CPW case
        private static bool RegPattern5(Node root, List<Tuple<string, string>> results)
        {
            var cur = root;
            if (cur is not null && cur.Ins == "CPW")
            {
                string src = cur.Args(0);
                string des = WindowsApp1.AddressSolver.Tune(cur.Args(1));
                int offset = (int)Math.Round(cur.Args(2) - 1f);
                results.Add(new Tuple<string, string>(src, des)); // The first one gets the name
                results.Add(new Tuple<string, string>("CPW", WindowsApp1.AddressSolver.AddrAdder(des, offset))); // the second one treated as an exception
                return true;
            }
            return false;
        }
    }
}