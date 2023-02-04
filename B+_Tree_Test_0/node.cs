using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Xml;
using System.IO;


namespace B__Tree_Test_0
{
    [DataContract]
    public class Node
    {
        public ulong id = 0;
        public Node parent { get; set; }
        public ulong[] values = new ulong[8];
        public Node[] keys = new Node[9];
        public bool check_leaf = true;
        public Node leftSibling { get; set; }
        public Node rightSibling { get; set; }
        public int nodeValueLenght = 8;
        public int nodeKeysLenght = 9;

        public string[] Data = new string[8];
    }
}
