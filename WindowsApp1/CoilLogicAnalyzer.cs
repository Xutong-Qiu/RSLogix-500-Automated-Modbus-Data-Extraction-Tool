using System;
using System.Collections.Generic;

namespace WindowsApp1
{
    /// <summary>
/// CoilLogicAnalyzer
/// This module analyzes the logic of the given parse tree and finds coil mapping
/// within it. It is based on logic patterns that have been seen in the PLC programs.
/// </summary>
    public static class CoilLogicAnalyzer
    {
        public static void FindCoilMapping(PLC_DB DB, Node root, List<Tuple<string, string>> results)
        {
            var cur = root;
            int bit = 0;
            if (cur is not null && cur.Ins == "EQU")
            {
                cur = cur.NextIns;
                if (cur is not null && cur.Ins == "XIO")
                {
                    cur = cur.NextIns;
                    if (cur.Ins == "BST")
                    {
                        if (CoilPatternSWP(cur, results))
                        {
                            return;
                        }
                        foreach (var branch in cur.Children)
                        {
                            if (CoilPatternIgnored(branch, results, bit))
                            {
                                continue;
                            }
                            if (CoilPattern1(branch, results, bit))
                            {
                                bit += 1;
                                continue;
                            }
                            if (CoilPattern2(DB, branch, results, bit))
                            {
                                bit += 1;
                                continue;
                            }
                            if (CoilPattern3(branch, results, bit))
                            {
                                bit += 1;
                                continue;
                            }
                            HandlesExceptionMapping(branch, results, bit);
                            bit += 1;
                        }
                    }
                }
            }
        }

        // MOV 0 to coil
        private static bool CoilPatternIgnored(Node root, List<Tuple<string, string>> results, int bit)
        {
            var cur = root;
            if (cur is not null && cur.Ins == "MOV")
            {
                return true;
            }
            return false;
        }

        // direct mapping case: XIC->OR
        private static bool CoilPattern1(Node root, List<Tuple<string, string>> results, int bit)
        {
            var cur = root;
            if (cur is not null && cur.Ins == "XIC")
            {
                string src = cur.Args[0];
                cur = cur.NextIns;
                if (cur is not null && cur.Ins == "OR")
                {
                    results.Add(new Tuple<string, string>(src, AddressSolver.Tune(cur.Args[0] + "/" + bit)));
                    return true;
                }
            }
            return false;
        }

        // EQU->OR
        private static bool CoilPattern2(PLC_DB DB, Node root, List<Tuple<string, string>> results, int bit)
        {
            var cur = root;
            if (cur is not null && cur.Ins == "EQU")
            {
                string src = cur.Args[1];
                if (DB.GetTagName(src).Contains("STATE"))
                {
                    src = cur.Args[0];
                }
                cur = cur.NextIns;
                if (cur is not null && cur.Ins == "OR")
                {
                    results.Add(new Tuple<string, string>(src, AddressSolver.Tune(cur.Args[0] + "/" + bit)));
                    return true;
                }
            }
            return false;
        }

        // NEQ->XIC->OR
        private static bool CoilPattern3(Node root, List<Tuple<string, string>> results, int bit)
        {
            var cur = root;
            if (cur is not null && cur.Ins == "NEQ")
            {
                cur = cur.NextIns;
                if (cur is not null)
                {
                    return CoilPattern1(cur, results, bit);
                }
            }
            return false;
        }

        // MOV/JSR/MOV/SWP
        private static bool CoilPatternSWP(Node root, List<Tuple<string, string>> results)
        {
            var cur = root;
            if (cur is not null && cur.Ins == "BST")
            {
                if (cur.Children.Count == 4)
                {
                    if (cur.Children[0].Ins == "MOV" && cur.Children[1].Ins == "JSR" && cur.Children[2].Ins == "MOV" && cur.Children[3].Ins == "SWP")
                    {
                        string src = AddressSolver.Tune(cur.Children[0].Args[0]);
                        string des = AddressSolver.Tune(cur.Children[2].Args[1]);
                        if ((src ?? "") != (des ?? ""))
                        {
                            for (int count = 0; count <= 15; count++)
                            {
                                string full_addr1 = src + "/" + count.ToString();
                                string full_addr2 = des + "/" + count.ToString();
                                results.Add(new Tuple<string, string>(full_addr1, full_addr2));
                            }
                        }
                        return true;
                    }
                }
            }
            return false;
        }


        // HandlesExceptionMapping
        private static bool HandlesExceptionMapping(Node root, List<Tuple<string, string>> results, int bit)
        {
            var cur = root;
            if (cur is not null && cur.Ins == "OR")
            {
                results.Add(new Tuple<string, string>("Logic involved", AddressSolver.Tune(cur.Args[0]) + "/" + bit));
                return true;
            }
            else if (cur is null)
            {
                return true;
            }
            else if (cur.Ins == "BST")
            {
                foreach (var branch in cur.Children)
                    HandlesExceptionMapping(branch, results, bit);
                HandlesExceptionMapping(cur.NextIns, results, bit);
            }
            else
            {
                HandlesExceptionMapping(cur.NextIns, results, bit);
            }
            return true;
        }


    }
}