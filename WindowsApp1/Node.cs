namespace WindowsApp1
{
    /// <summary>
    /// The instances of this class represents nodes in the parse tree.
    /// A node is essientially an instruction.
    /// </summary>
    /// <remarks>
    /// Ins: represents the instruction represented by this node.
    /// NextIns: pointer points to next node
    /// Children: is not empty if only if the node is a branch instruction(BST). 
    ///             Each element in children reprents a brnach in BST instruction.
    /// Args: arugments of the current instruction
    /// </remarks>
    public class Node
    {
        public string Ins { get; set; }
        public List<Node> Children { get; set; }
        public Node NextIns { get; set; }
        public List<string> Args { get; set; }
        public Node(string Str)
        {
            Ins = Str;
            Children = new List<Node>();
            Args = new List<string>();
            NextIns = null;
        }
        public override string ToString()
        {
            string s = "";
            if (Ins == "BST")
            {
                s = Environment.NewLine;
                s += "BST{" + Environment.NewLine;
                foreach (var child in Children)
                {
                    string temp;
                    temp = child.ToString();
                    // temp = temp.Replace(Environment.NewLine, Environment.NewLine & "    ")  'add indent
                    s += "  " + temp;
                    s += Environment.NewLine;
                }
                s += "}";
            }
            else
            {
                s += Ins + "(";
                foreach (var arg in Args)
                    s += arg + ", ";
                s = s.Substring(0, s.Length - 2) + ")" + " ";
            }

            if (NextIns is not null)
            {
                s += NextIns.ToString();
            }
            return s;
        }
    }
}