using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace B__Tree_Test_0
{
    class Program
    {
        public static Node GLOBALMAINROOT = new Node();
        public static string MAINFILEPATH = "~\\DataBase\\"; //Path to database folder

        static void Main(string[] args)
        {

            using (StreamReader sr = File.OpenText(MAINFILEPATH + "_globalNodeID.txt"))
            {
                string id = sr.ReadLine().Split(",")[0];
                if (id != "null") { GLOBALMAINROOT = fileObjectification(Convert.ToUInt64(id), 0); }
                else
                {
                    GLOBALMAINROOT = new Node();
                }
            }

            // we store the id of the global node
            using (FileStream fs = File.Create(MAINFILEPATH + "_globalNodeID.txt"))
            {
                byte[] info = new UTF8Encoding(true).GetBytes(Convert.ToString(GLOBALMAINROOT.id));
                fs.Write(info, 0, info.Length);
            }

            Console.WriteLine();
            Console.Write("Last Result");
            PrintTree(GLOBALMAINROOT);
            Console.WriteLine();
        }

        private static void InsertValue(ulong sayı)
        {
            //find the node to add
            Node CN = SearchNode(sayı); //the original node to be found
            //check node occupancy
            if (findLength(CN.values) != CN.nodeValueLenght) LeafNodeInsert(CN, sayı); //insert directly if node is empty
            else //node doluysa 
            {
                //try to give right or left//
                //if there is space on the right
                if (CN.leftSibling != null && findLength(CN.leftSibling.values) != CN.leftSibling.nodeValueLenght)
                {
                    ulong lowestValue = PopLowestValue(CN, sayı);
                    if (sayı == lowestValue) LeafNodeInsert(CN.leftSibling, lowestValue); //Control to move data too
                    else 
                    {
                        LeafNodeInsert(CN.leftSibling, lowestValue);
                        int currentLocation;
                        try
                        {
                            currentLocation = GetDataIDinLeafNode(CN.leftSibling, lowestValue); //finds the location of the data in the leafnode
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("The requested id does not exist on the leaf node.");
                            throw;
                        }
                        if (CN.leftSibling.Data[currentLocation] == null) CN.leftSibling.Data[currentLocation] = PopLowestData(CN);
                        else SlideDataInfos(CN.leftSibling, currentLocation, PopLowestData(CN));
                    }
                    RootValueReset(CN.parent);
                }
                //if there is space on the right
                else if (CN.rightSibling != null && findLength(CN.rightSibling.values) != CN.rightSibling.nodeValueLenght)
                {
                    ulong largestValue = PopLargestValue(CN, sayı);
                    if (sayı == largestValue) LeafNodeInsert(CN.rightSibling, largestValue); //Control to move data too
                    else
                    {
                        LeafNodeInsert(CN.rightSibling, largestValue); //passing data and id thrown to the right
                        int currentLocation;
                        try
                        {
                            currentLocation = GetDataIDinLeafNode(CN.rightSibling, largestValue); //finds the location of the data in the leafnode
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("The requested id does not exist on the leaf node.");
                            throw;
                        }
                        if (CN.rightSibling.Data[currentLocation] == null) CN.rightSibling.Data[currentLocation] = PopLargestData(CN);
                        else SlideDataInfos(CN.rightSibling, currentLocation, PopLargestData(CN));
                    }
                    LeafNodeInsert(CN.rightSibling, sayı);
                    RootValueReset(CN.parent);
                }
                //split the leaf node into two
                else
                {
                    SplitLeaf(CN, sayı);
                }
            }
            HigherParentKeyReset(CN);
            FilingTheConnectedNodeObjects(CN, true);
        }

        //Split leaf due to new incoming and overflowing number
        private static void SplitLeaf(Node leafnode, ulong number)
        {
            Node newLeaf = new Node();
            //brings the new array together in the linked list
            newLeaf.parent = leafnode.parent;
            newLeaf.rightSibling = leafnode.rightSibling;
            if(newLeaf.rightSibling != null) newLeaf.rightSibling.leftSibling = newLeaf;
            leafnode.rightSibling = newLeaf;
            newLeaf.leftSibling = leafnode;

            number = PopLargestValue(leafnode, number);
            LeafNodeInsert(newLeaf, number);

            Dictionary<ulong, string> dataDict = new Dictionary<ulong, string>();
            for (int i = 0; i < leafnode.nodeValueLenght; i++)
            {
                dataDict.Add(leafnode.values[i], leafnode.Data[i]);
            }

            for (int i = (leafnode.nodeValueLenght / 2); i < leafnode.nodeValueLenght; i++)
            {
                LeafNodeInsert(newLeaf, leafnode.values[i]);
                leafnode.values[i] = 0;
            }
            //leafs split

            for (int i = 0; i < newLeaf.nodeValueLenght; i++)
            {
                if (leafnode.values[i] == 0) leafnode.Data[i] = null;
                if(newLeaf.values[i] != number && newLeaf.values[i] != 0) newLeaf.Data[i] = dataDict[newLeaf.values[i]];
            }

            if (leafnode.parent != null) 
            {
                if (findLength(leafnode.parent.values) != leafnode.parent.nodeValueLenght) //İf the parent is empty
                {
                    for (int i = 0; i < leafnode.parent.nodeValueLenght; i++)
                    {
                        if (leafnode.parent.keys[i] == leafnode)
                        {
                            AddChild(leafnode.parent, newLeaf, i + 1);
                            RootValueReset(leafnode.parent);
                            break;
                        }
                    }
                }
                else // if the parent that needs to be found is full
                {
                    //should look at the right and left parentes and send them there if there is a place
                    //if there is an empty space on the left
                    if (leafnode.parent.leftSibling != null && findLength(leafnode.parent.leftSibling.values) != leafnode.parent.leftSibling.nodeValueLenght)
                    {
                        newLeaf = PopLowestNode(leafnode.parent, newLeaf);

                        for (int i = 0; i < leafnode.parent.leftSibling.nodeValueLenght+1; i++) //find the first empty space on the left
                        {
                            if (leafnode.parent.leftSibling.keys[i] == null)
                            {
                                AddChild(leafnode.parent.leftSibling, newLeaf, i);
                                RootValueReset(leafnode.parent.leftSibling);
                                break;
                            }
                        }
                        //refresh parent links after link switching
                        HigherParentKeyReset(leafnode.parent.leftSibling);
                    }
                    //if there is an empty space on the right
                    else if (leafnode.parent.rightSibling != null && findLength(leafnode.parent.rightSibling.values) != leafnode.parent.rightSibling.nodeValueLenght)
                    {
                        newLeaf = PopHighestNode(leafnode.parent, newLeaf);

                        AddChild(leafnode.parent.rightSibling, newLeaf, 0);
                        RootValueReset(leafnode.parent.rightSibling);

                        //refresh parent links after link switching
                        HigherParentKeyReset(leafnode.parent.rightSibling);
                    }
                    //Split Parent
                    else
                    {
                        if (leafnode.parent.values[(leafnode.parent.nodeValueLenght / 2)] < newLeaf.values[0]) //if new node is greater than general
                        {
                            SplitParent(leafnode.parent, newLeaf, true);
                        }
                        else //if the new node is smaller than the general
                        {
                            SplitParent(leafnode.parent, newLeaf, false);
                        }
                        //refresh parent links after link switching
                        HigherParentKeyReset(leafnode);
                    }
                }
            }
            else //if there is no parent
            {
                Node newRoot = new Node();
                newRoot.check_leaf = false;

                if (leafnode == GLOBALMAINROOT) GLOBALMAINROOT = newRoot;

                newRoot.keys[0] = leafnode;
                leafnode.parent = newRoot;

                newRoot.keys[1] = newLeaf;
                newLeaf.parent = newRoot;

                RootValueReset(newRoot);
            }
        }

        //Split parent due to new incoming and overflowing node
        private static void SplitParent(Node oldRoot, Node newLeaf, Boolean addRight)
        {
            //establishing connections
            Node newRoot = new Node();
            newRoot.check_leaf = false;

            if (newRoot.rightSibling != null) { newRoot.rightSibling.leftSibling = newRoot; }
            newRoot.rightSibling = oldRoot.rightSibling;

            oldRoot.rightSibling = newRoot;
            newRoot.leftSibling = oldRoot;


            //added to the new or old node according to the extra leafnode value(above or below average)
            if (addRight) 
            {
                //filling the children of the new parent
                int rootPointer = 0;
                for (int i = (oldRoot.nodeValueLenght / 2) + 1; i < oldRoot.nodeKeysLenght; i++)
                {
                    newRoot.keys[rootPointer] = oldRoot.keys[i];
                    newRoot.keys[rootPointer].parent = newRoot;
                    oldRoot.keys[i] = null;
                    rootPointer++;
                }
                newRoot.keys[rootPointer] = newLeaf; newLeaf.parent = newRoot; 
            }
            else 
            {
                //filling the children of the new parent
                int rootPointer = 0;
                for (int i = (oldRoot.nodeValueLenght / 2); i < oldRoot.nodeKeysLenght; i++) //division should be optimized
                {
                    newRoot.keys[rootPointer] = oldRoot.keys[i];
                    newRoot.keys[rootPointer].parent = newRoot;
                    oldRoot.keys[i] = null;
                    rootPointer++;
                }
                RootValueReset(oldRoot); //refresh parent links after link switching
                AddChild(oldRoot, newLeaf, 0); newLeaf.parent = oldRoot; 
            }

            RootKeyReset(oldRoot);
            RootKeyReset(newRoot);

            RootValueReset(oldRoot);
            RootValueReset(newRoot);

            if (oldRoot.parent != null) //if there is any parent
            {
                if (findLength(oldRoot.parent.values) != oldRoot.parent.nodeValueLenght) //if the parent is not full
                {
                    for (int i = 0; i < oldRoot.parent.nodeValueLenght; i++)
                    {
                        if (oldRoot.parent.keys[i] == oldRoot)
                        {
                            AddChild(oldRoot.parent, newRoot, i + 1);
                            break;
                        }
                    }
                    HigherParentKeyReset(oldRoot.parent);
                }
                else
                {
                    if (oldRoot.values[(oldRoot.nodeValueLenght / 2) - 1] < newLeaf.values[0]) //if new node is greater than general
                    {
                        SplitParent(oldRoot.parent, newRoot, true);
                    }
                    else //if the new node is smaller than the general
                    {
                        SplitParent(oldRoot.parent, newRoot, false);
                    }
                    HigherParentKeyReset(oldRoot.parent); //refresh parent links after link switching
                }
            }
            else //if there is no parent
            {
                Node newParentRoot = new Node();
                newParentRoot.check_leaf = false;

                newParentRoot.keys[0] = oldRoot;
                oldRoot.parent = newParentRoot;

                newParentRoot.keys[1] = newRoot;
                newRoot.parent = newParentRoot;

                GLOBALMAINROOT = newParentRoot;

                RootValueReset(newParentRoot);
            }
        }

        //Adds child nodes by shifting
        private static void AddChild(Node parent, Node newLeaf, int keyPoint)  
        {
            if (parent.keys[keyPoint] == null) //if the part to be inserted is empty
            { 
                parent.keys[keyPoint] = newLeaf;
                newLeaf.parent = parent;
            } 
            else
            {
                //If there is an empty space in the parent, if the part to be inserted is full, the shift operation
                Node tempNode1 = parent.keys[keyPoint];
                parent.keys[keyPoint] = newLeaf;
                while (tempNode1 != null)
                {
                    keyPoint++;
                    Node tempNode2 = parent.keys[keyPoint];
                    parent.keys[keyPoint] = tempNode1;
                    tempNode1 = tempNode2;
                }
            }
        }

        //Refresh parent links after link switching
        private static void HigherParentKeyReset(Node node) 
        {            
            while (node.parent != null)
            {
                RootValueReset(node.parent);
                node = node.parent;
            }
        }

        //Add the given leafnode to the parent and remove the largest node
        private static Node PopHighestNode(Node parent, Node newleaf) 
        {
            Node highest = newleaf;
            for (int i = 0; i < parent.nodeValueLenght + 1; i++) //subtracts the largest by shifting
            {
                if (parent.keys[i] != null && highest.values[0] < parent.keys[i].values[0])
                {
                    Node temp_node = highest;
                    highest = parent.keys[i];
                    parent.keys[i] = temp_node;
                }
            }
            RootValueReset(parent);

            return highest;
        }

        //Add the given leafnode to the parent and remove the smallest node (the node with the lowest values)
        private static Node PopLowestNode(Node parent, Node newleaf)
        {
            Node lowest = newleaf;
            for (int i = parent.nodeValueLenght; i > 0 - 1; i--) //subtracts the smallest by shifting
            {
                if (parent.keys[i] != null && lowest.values[0] > parent.keys[i].values[0])
                {
                    Node temp_node = lowest;
                    lowest = parent.keys[i];
                    parent.keys[i] = temp_node;
                }
            }
            RootValueReset(parent);

            return lowest;
        }

        //Add the given number to the node get the maximum
        private static ulong PopLargestValue(Node leafNode, ulong sayı) 
        {
            ulong lastIndex = Convert.ToUInt64(leafNode.nodeValueLenght - 1);
            if (leafNode.values[lastIndex] > sayı)
            {
                ulong temp = leafNode.values[lastIndex];
                leafNode.values[lastIndex] = sayı;
                LeafNodeSort(leafNode);
                return temp;
            }
            else
            {
                return sayı;
            }
        }

        //Add the given number to the node and subtract the smallest
        private static ulong PopLowestValue(Node leafNode, ulong sayı) 
        {
            if (leafNode.values[0] < sayı)
            {
                ulong temp = leafNode.values[0];
                leafNode.values[0] = sayı;
                LeafNodeSort(leafNode);
                return temp;
            }
            else
            {
                return sayı;
            }
        }

        //Id returns the smallest data information and shifts the data.
        private static string PopLowestData(Node leafNode) 
        {
            string lowest = leafNode.Data[0];
            leafNode.Data[0] = null;
            for (int i = 0; i < leafNode.nodeValueLenght - 1; i++)
            {
                if (leafNode.Data[i + 1] != null)
                {
                    string temp_data = leafNode.Data[i + 1];
                    leafNode.Data[i] = temp_data;                    
                }
            }
            return lowest;
        }

        //Id returns the largest data information and shifts the data
        private static string PopLargestData(Node leafNode)  
        {
            string largest = leafNode.Data[leafNode.nodeValueLenght-1]; //the largest naturally last id
            leafNode.Data[leafNode.nodeValueLenght - 1] = null;
            return largest;
        }

        //Refreshes the values of parent nodes
        private static void RootValueReset(Node rootNode) 
        {
            if (rootNode.keys[0].check_leaf == true)
            {
                for (int i = 0; i < rootNode.values.Length; i++)
                {
                    if (rootNode.keys[i + 1] != null)
                    {
                        rootNode.values[i] = rootNode.keys[i + 1].values[0];
                    }
                    else rootNode.values[i] = 0;
                }
            }
            else //resets the top level ( >2) parent's values
            {
                for (int i = 0; i < rootNode.nodeValueLenght; i++)
                {
                    if (rootNode.keys[i + 1] != null)
                    {
                        Node temp_node = rootNode.keys[i + 1]; //reaches the deepest node and refreshes
                        while (true)
                        {
                            if (temp_node.check_leaf == true)
                            {
                                rootNode.values[i] = temp_node.values[0];
                                break;
                            }
                            temp_node = temp_node.keys[0];
                        }
                    }
                    else rootNode.values[i] = 0;
                }
            }
        }

        //Sort root's nodes by size using bubble sort
        private static void RootKeyReset(Node rootNode) 
        {
            int n = rootNode.nodeKeysLenght; //bubble sort
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - i - 1; j++)
                {
                    if (rootNode.keys[j + 1] != null && rootNode.keys[j].values[0] > rootNode.keys[j + 1].values[0])
                    {
                        Node temp = rootNode.keys[j]; //swap
                        rootNode.keys[j] = rootNode.keys[j + 1];
                        rootNode.keys[j + 1] = temp;
                    }
                }
            }
        }

        //Inserts if there is empty space in the leaf node
        private static void LeafNodeInsert(Node leafnode, ulong sayı)
        {
            for (int j = 0; j < leafnode.nodeValueLenght; j++)
            {
                if (leafnode.values[j] == 0) { leafnode.values[j] = sayı; break; }
            }
            LeafNodeSort(leafnode);
        }

        //Returns the number of non-zero values of the incoming array
        private static int findLength(ulong[] arr)
        { 
            int len = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] != 0) len++;
            }
            return len;
        }

        //Sorts the LeafNode from smallest to largest //should be optimize
        private static void LeafNodeSort(Node leafnode) 
        {
            ulong[] temp_array = new ulong[leafnode.nodeValueLenght];
            int count = 0;

            for (int i = 0; i < leafnode.nodeValueLenght; i++)
            {
                if (leafnode.values[i] != 0) { temp_array[count] = leafnode.values[i]; count++; }
            }

            Array.Clear(leafnode.values, 0, leafnode.nodeValueLenght);
            Array.Sort(temp_array);

            count = 0;

            for (int i = 0; i < leafnode.nodeValueLenght; i++)
            {
                if (temp_array[i] != 0) { leafnode.values[count] = temp_array[i]; count++; }
            }
        }

        //Prints Node for development testing
        private static void PrintNode(Node node)
        {
            for (int i = 0; i < node.values.Length; i++)
            {
                Console.Write(i);
                Console.Write(": ");
                Console.WriteLine(node.values[i]);
            }
        }

        //Returns the leaf node where the value is located (in file scope)
        //Reads and objectifies all nodes it passes through during the search process
        private static Node SearchNode(ulong value)  
        {
            Node CN = GLOBALMAINROOT; //CN: current node
            while (CN.check_leaf == false) //go down until you get to leaf
            {
                for (int i = 0; i < CN.values.Length; i++)
                {
                    if (value == CN.values[i])
                    {
                        CN.keys[i + 1] = fileObjectification(CN.keys[i + 1].id, 0);
                        CN.keys[i + 1].parent = CN;
                        CN = CN.keys[i + 1];
                        break;
                    }
                    else if (value < CN.values[i])
                    {
                        CN.keys[i] = fileObjectification(CN.keys[i].id, 0);
                        CN.keys[i].parent = CN;
                        CN = CN.keys[i];
                        break;
                    }
                    else if (findLength(CN.values) == i + 1)
                    {
                        CN.keys[i + 1] = fileObjectification(CN.keys[i + 1].id, 0);
                        CN.keys[i + 1].parent = CN;
                        CN = CN.keys[i + 1];
                        break;
                    }
                }
            }
            return CN;
        }

        //Returns the leaf node where the value is located (in object scope)
        private static Node SearchNodeInObject(ulong value) 
        {
            Node CN = GLOBALMAINROOT; //CN: current node

            while (CN.check_leaf == false) //go down until you get to leaf
            {
                for (int i = 0; i < CN.values.Length; i++)
                {
                    if (value == CN.values[i])
                    {
                        CN = CN.keys[i + 1];
                        break;
                    }
                    else if (value < CN.values[i])
                    {
                        CN = CN.keys[i];
                        break;
                    }
                    else if (findLength(CN.values) == i + 1)
                    {
                        CN = CN.keys[i + 1];
                        break;
                    }
                }
            }
            return CN;
        }

        //Searches for data in incoming LeafNode and returns Boolean
        private static Boolean FindInLeafNode(ulong value, Node leafNode)
        {
            for (int i = 0; i < leafNode.nodeValueLenght; i++)
            {
                if (leafNode.values[i] == value) return true;
            }
            return false;
        }

        //Prints the created tree structure to the console
        //for development testing
        private static void PrintTree(Node root)
        {
            Console.WriteLine("------------------------------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine("Start of Tree");
            Console.WriteLine();

            int counter = 0;
            Console.Write("{0}:   ",counter++);
            PrintValues(root);
            Console.WriteLine();

            Node[] temp = new Node[512];
            Node[] temp2 = new Node[512];

            int temp_len_counter = 0;
            int temp2_len_counter = 0;
            for (int i = 0; i < root.nodeValueLenght + 1; i++)
            {
                if (root.keys[i] != null) { temp[i] = root.keys[i]; temp_len_counter++; }
            }

            do
            {
                Console.Write("{0}:   ", counter++);
                for (int i = 0; i < temp_len_counter; i++)
                {
                    PrintValues(temp[i]);
                    for (int j = 0; j < temp[i].keys.Length; j++)
                    {
                        if (temp[i].keys[j] != null) 
                        {
                            for (int k = 0; k < temp.Length; k++)
                            {
                                if (temp2[k] == null) { temp2[k] = temp[i].keys[j]; break; }
                            }
                            temp2_len_counter++; 
                        }
                    }
                }
                Console.WriteLine();
                temp_len_counter = temp2_len_counter;
                temp = temp2;
                temp2 = new Node[512];
                temp2_len_counter = 0;

            } while (temp[0] != null);

            Console.WriteLine();
            Console.WriteLine("End of Tree");
            Console.WriteLine("-----------------------------------------------------------------------------------");
            Console.WriteLine();
        }

        //Prints the values in the node to the console
        //for development testing
        private static void PrintValues(Node node)
        {
            for (int i = 0; i < node.nodeValueLenght; i++)
            {
                if (node.values[i] != 0) Console.Write(node.values[i]);
                else continue;
                if (i == node.nodeValueLenght - 1) break;
                Console.Write(" , ");
            }
            Console.Write(" | ");
        }

        //Takes the table name and column names from the user and adds them to the _indexes.txt file
        public static void InsertNewTable(string tableName, string rowNames)
        {
            ulong tableID = 0;
            try
            {
                ulong lastId;
                using (StreamReader sr = File.OpenText(MAINFILEPATH + "_tableOtoincrement.txt"))
                {
                    lastId = Convert.ToUInt64(sr.ReadLine());
                    lastId += 100;
                    tableID = lastId;
                }
                using (FileStream fs = File.Create(MAINFILEPATH + "_tableOtoincrement.txt"))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(Convert.ToString(lastId));
                    fs.Write(info, 0, info.Length);
                }
            }
            catch (Exception ex)
            {
                Console.Write("When Insert New Table id: ");
                Console.WriteLine(ex.ToString());
            }

            File.AppendAllTextAsync(MAINFILEPATH + "_indexes.txt",
                Convert.ToString(tableID) + "," +  tableName + ",0|id,foreign key," + rowNames + Environment.NewLine);
        }

        //Get table name and delete all table files
        //Edits the information of the _indexes.txt file
        public static void DeleteTable(string[,] tables, string tableName)
        {
            //
        }

        //Gets data id in leafnode
        public static int GetDataIDinLeafNode(Node node, ulong id)
        {
            for (int i = 0; i < node.nodeValueLenght; i++)
            {
                if (node.values[i] == id) { return i; }
            }
            return 999999; //Returns a value that will never happen and make it fail
        }

        //Adds new data to leafnode by shifting
        public static void SlideDataInfos(Node node, int leafNodeId, string newData) 
        {
            String tempData1 = node.Data[leafNodeId];
            node.Data[leafNodeId] = newData;
            while (tempData1 != null && leafNodeId < node.nodeValueLenght - 1)
            {
                leafNodeId++;
                String tempData2 = node.Data[leafNodeId];
                node.Data[leafNodeId] = tempData1;
                tempData1 = tempData2;
            }
        }

        //It is called with the table variable containing the table information
        //prints all the information of the table name it will write to the console
        public static void SELECT(String[,] tables)
        {
            //Kullanıcı tablo yazdırıyor
            try
            {
                Console.Write(" : ");
                String chosen_table_name = Console.ReadLine();
                for (int i = 0; i < tables.Length / 2; i++)
                {
                    if (tables[i, 1] == chosen_table_name)
                    {
                        string chosen_table_id = tables[i, 0];
                        for (int j = 0; j < tables[i, 3].Split(",").Length; j++)
                        {
                            Console.Write(tables[i, 3].Split(",")[j]);
                            Console.Write("\t\t");
                        }
                        Console.WriteLine();
                        printTableRows(Convert.ToUInt64(tables[i, 0]), Convert.ToInt32(tables[i, 2])); //print table rows
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write("When SELECT: ");
                Console.WriteLine(ex.ToString());
            }
        }

        //reads the given number of data from the given id
        public static void printTableRows(ulong id, int numberOfRow) 
        {
            Node CN = SearchNode(id); //Current Node
            int LeafNodeID;
            try
            {
                LeafNodeID = GetDataIDinLeafNode(CN, id); //finds the location of the data in the leafnode
            }
            catch (Exception)
            {
                Console.WriteLine("The requested id does not exist on the leaf node.");
                throw;
            }
            
            for (int i = 0; i < numberOfRow; i++)
            {
                if (CN.Data[LeafNodeID] != null && LeafNodeID != CN.nodeValueLenght)
                {
                    Console.Write(id);
                    Console.Write(":\t\t");
                    Console.Write(i);
                    Console.Write("\t\t");

                    //data is converted to path and data is read from the file
                    Console.WriteLine(File.ReadAllText(MAINFILEPATH + CN.Data[LeafNodeID])); 
                    id++;
                    LeafNodeID++;
                }
                else //when the node is full, it switches to the indexed node (to the right of it)
                {
                    CN = fileObjectification(CN.rightSibling.id, 0);  
                    LeafNodeID = 0;
                    Console.Write(id);
                    Console.Write(":\t\t");
                    Console.Write(i);
                    Console.Write("\t\t");

                    //data is converted to path and data is read from the file
                    Console.WriteLine(File.ReadAllText(MAINFILEPATH + CN.Data[LeafNodeID]));
                    id++;
                    LeafNodeID++;
                }
            }
        }

        //inserts data and the id of the node to which the data will be inserted
        public static void InsertData(ulong id, string data) 
        {
            Node node = SearchNode(id); //searches for the node to be found
            if (FindInLeafNode(id, node) == false) //if the value to be added does not already exist
            {
                string dataFilePath = Convert.ToString(GiveID()) + ".txt";
                using (FileStream fs = File.Create(MAINFILEPATH + dataFilePath))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(data);
                    fs.Write(info, 0, info.Length);
                }
                InsertValue(id); //inserts data in object scope
                node = SearchNode(id); //searches in files again, sends object
                int currentLocation;
                try
                {
                    currentLocation = GetDataIDinLeafNode(node, id); //finds the location of the data in the leaf node
                }
                catch (Exception)
                {
                    Console.WriteLine("The requested id does not exist on the leaf node.");
                    throw;
                }
                if (node.Data[currentLocation] == null) node.Data[currentLocation] = dataFilePath;
                else SlideDataInfos(node, currentLocation, dataFilePath);
                HigherParentKeyReset(node); //saves changes in object scope
                objectFiling(node); //saves all changes in file scope
                //FilingTheConnectedNodeObjects(node);
            }
            else
            {
                Console.WriteLine("This ID is already exist: " + Convert.ToString(id));
            }
        }

        //Adds the selected data to the selected table in a single row
        //With the table variable that holds the table information inside
        public static void INSERT(string[,] tables,string tableName, string data)
        {
            for (int i = 0; i < tables.Length / 2; i++)
            {
                if (tables[i, 1] == tableName)
                {
                    ulong id = Convert.ToUInt64(tables[i, 0]) + Convert.ToUInt64(tables[i, 2]);
                    InsertData(id, data); //add data and id

                    //information about the tables is kept in the _indexs.txt file, we record the changes made
                    String line;
                    using (var sr = new StreamReader(MAINFILEPATH + "_indexes.txt"))
                    {
                        for (int j = 1; j < i; j++) { sr.ReadLine(); }
                        line = sr.ReadLine();
                    }
                    String newLine = line.Split('|')[0].Split(',')[0] + ',' +
                                     line.Split('|')[0].Split(',')[1] + ',' +
                                     Convert.ToString(Convert.ToUInt64(line.Split('|')[0].Split(',')[2]) + 1)
                                     + '|' + line.Split('|')[1];

                    string[] lines = File.ReadAllLines(MAINFILEPATH + "_indexes.txt");
                    using (StreamWriter writer = new StreamWriter(MAINFILEPATH + "_indexes.txt"))
                    {
                        for (int j = 0; j <= lines.Length-1; ++j)
                        {
                            if (j == i) //write when you find the line to edit
                            {
                                writer.WriteLine(newLine);
                            }
                            else
                            {
                                writer.WriteLine(lines[j - 1]);
                            }
                        }
                    }
                    //increased the index 1
                    break;
                }
            }
        }

        //Deletes the data in the node with the selected node id
        public static void DELETE(ulong id)
        {
            Node node = SearchNode(id); //searchs for the node where the data is located
            Dictionary<ulong, string> dataDict = new Dictionary<ulong, string>();
            for (int i = 0; i < node.nodeValueLenght; i++)
            {
                dataDict.Add(node.values[i], node.Data[i]);
            }
            int currentLocation;
            try
            {
                currentLocation = GetDataIDinLeafNode(node, id); //finds the location of the data in the leafnode
            }
            catch (Exception)
            {
                Console.WriteLine("The requested id does not exist on the leaf node.");
                throw;
            }
            node.Data[currentLocation] = null;
            node.values[currentLocation] = 0;
            LeafNodeSort(node);

            int dataCurrentLocation = 0;
            foreach (ulong item in node.values) //for floating value values organizes the data according to the shift
            {
                if (node.Data[dataCurrentLocation] != null ) node.Data[dataCurrentLocation] = dataDict[item];
                dataCurrentLocation++;
            }
            objectFiling(node);
        }

        //Replaces the data in the node with the selected node id
        public static void UPDATE(ulong id, string data)
        {
            Node node = SearchNode(id); //searchs for the node where the data is located
            int currentLocation;
            try
            {
                currentLocation = GetDataIDinLeafNode(node, id); //finds the location of the data in the leafnode
            }
            catch (Exception)
            {
                Console.WriteLine("The requested id does not exist on the leaf node.");
                throw;
            }
            using (FileStream fs = File.Create(MAINFILEPATH + node.Data[currentLocation]))
            {
                byte[] info = new UTF8Encoding(true).GetBytes(data);
                fs.Write(info, 0, info.Length);
            }
        }

        //Extracts information from the file and returns an object(node)
        //Connects with neighborhood links
        //If neighborhoodDegree < 2 get also the nodes it is connected to
        // id,parent,leftsibling,rightsibling,check_leaf|value|keys
        public static Node fileObjectification(ulong id, int neighborhoodDegree) 
        {   
            Node node = new Node();
            try
            {
                using (StreamReader sr = File.OpenText(MAINFILEPATH + Convert.ToString(id) + ".txt"))
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        node.id = Convert.ToUInt64(s.Split("|")[0].Split(",")[0]);
                        if (s.Split("|")[0].Split(",")[1] != "null" && neighborhoodDegree < 2) 
                        { 
                            node.parent = fileObjectification(Convert.ToUInt64(s.Split("|")[0].Split(",")[1]), neighborhoodDegree); 
                        }
                        if (s.Split("|")[0].Split(",")[2] != "null" && neighborhoodDegree < 2) 
                        { 
                            node.leftSibling = fileObjectification(Convert.ToUInt64(s.Split("|")[0].Split(",")[2]), neighborhoodDegree + 1); 
                        }
                        if (s.Split("|")[0].Split(",")[3] != "null" && neighborhoodDegree < 2) 
                        { 
                            node.rightSibling = fileObjectification(Convert.ToUInt64(s.Split("|")[0].Split(",")[3]), neighborhoodDegree + 1); 
                        }
                        
                        node.check_leaf = Convert.ToBoolean(s.Split("|")[0].Split(",")[4]);

                        for (int i = 0; i < node.nodeValueLenght; i++)
                        {
                            node.values[i] = Convert.ToUInt64(s.Split("|")[1].Split(",")[i]);
                        }
                        for (int i = 0; i < node.nodeKeysLenght; i++)
                        {
                            if (s.Split("|")[2].Split(",")[i] != "null")
                            {
                                node.keys[i] = fileObjectification(Convert.ToUInt64(s.Split("|")[2].Split(",")[i]), neighborhoodDegree + 1);
                                node.keys[i].id = Convert.ToUInt64(s.Split("|")[2].Split(",")[i]);
                            }
                        }
                        for (int i = 0; i < node.nodeValueLenght; i++)
                        {
                            if (s.Split("|")[3].Split(",")[i] != "null")
                            {
                                node.Data[i] = s.Split("|")[3].Split(",")[i];
                            }
                            else
                            {
                                node.Data[i] = null;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write("When objectify: ");
                Console.WriteLine(ex.ToString());
            }
            return node;
        }

        //Main function of save to file for node(object) that arrives with specified syntax
        public static void objectFiling(Node node)
        {
            if (node.id == 0) SetID(node);
            using (FileStream fs = File.Create(MAINFILEPATH + Convert.ToString(node.id) + ".txt"))
            {
                string s = "";
                s += Convert.ToString(node.id);
                if (node.parent != null)
                {
                    if (node.parent.id == 0) SetID(node.parent);
                    s += "," + Convert.ToString(node.parent.id);
                }
                else s += ",null";
                if (node.leftSibling != null) 
                { 
                    if (node.leftSibling.id == 0) SetID(node.leftSibling); 
                    s += "," + Convert.ToString(node.leftSibling.id); 
                }
                else s += ",null";
                if (node.rightSibling != null) 
                {
                    if (node.rightSibling.id == 0) SetID(node.rightSibling);
                    s += "," + Convert.ToString(node.rightSibling.id); 
                }
                else s += ",null";
                s += "," + Convert.ToString(node.check_leaf) + "|";

                for (int i = 0; i < node.nodeValueLenght; i++)
                {
                    s += Convert.ToString(node.values[i]);
                    if (node.nodeValueLenght - i != 1) s += ",";
                }
                s += "|";

                for (int i = 0; i < node.nodeKeysLenght; i++)
                {
                    if (node.keys[i] != null) { s += Convert.ToString(node.keys[i].id); }
                    else s += "null";
                    if (node.nodeKeysLenght - i != 1) s += ",";
                }
                s += "|";

                for (int i = 0; i < node.nodeValueLenght; i++)
                {
                    if (node.Data[i] != null)
                    {
                        s += node.Data[i];
                    }
                    else
                    {
                        s += "null";
                    }
                    if (node.nodeValueLenght - i != 1) s += ",";
                }

                byte[] info = new UTF8Encoding(true).GetBytes(s);
                fs.Write(info, 0, info.Length);
            }
        }

        //Sends all nodes which linked and information is likely to change to be saved in files again
        //Bool main: check to update only nodes around the changed node
        public static void FilingTheConnectedNodeObjects(Node node, bool main) 
        {
            if (node.id == 0) {
                //if the node's id is zero it is not a main node
                SetID(node); FilingTheConnectedNodeObjects(node, false); 
            }

            objectFiling(node);
            if (node.leftSibling != null && main) { objectFiling(node.leftSibling); }
            if (node.rightSibling != null && main) { objectFiling(node.rightSibling); }
            FilingHigherParents(node);
            if (node.leftSibling != null && main && node.leftSibling.parent != node.parent) { FilingHigherParents(node.leftSibling); }
            if (node.rightSibling != null && main && node.rightSibling.parent != node.parent) { FilingHigherParents(node.rightSibling); }
        }

        //Using the parent connections of the nodes,
        //Predicting that the top of the tree will also change,
        //Sends the parent nodes to save them in files
        public static void FilingHigherParents(Node node)
        {
            while (node.parent != null)
            {
                objectFiling(node.parent);
                node = node.parent;
            }
        }

        //Defines autoincrement id to node
        public static void SetID(Node node) 
        {
            try
            {
                node.id = GiveID();
            }
            catch (Exception ex)
            { 
                Console.Write("When setting id: ");
                Console.WriteLine(ex.ToString());
            }
        }

        //Takes and increments the last unique number from file to any node
        public static ulong GiveID() 
        {
            ulong id = 0;
            try
            {
                ulong lastId;
                using (StreamReader sr = File.OpenText(MAINFILEPATH + "_otoincrement.txt"))
                {
                    lastId = Convert.ToUInt64(sr.ReadLine());
                    lastId += 1;
                    id = lastId;
                }
                using (FileStream fs = File.Create(MAINFILEPATH + "_otoincrement.txt"))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(Convert.ToString(lastId));
                    fs.Write(info, 0, info.Length);
                }
            }
            catch (Exception ex)
            {
                Console.Write("When giving id: ");
                Console.WriteLine(ex.ToString());
            }
            return id;
        }

        //Saves table information from index file to two dimensional array
        //array information:
        //0: id
        //1: table name
        //2: number of row
        //3: column names
        //Prints tables to console if consoleWrite with bool variable is given as "true" 
        public static string[,] getTableInformations(bool consoleWrite)
        {
            Console.WriteLine();
            //the maximum possible number of tables is selected as 1000
            String[,] tables = new string[1000, 4];
            try
            {
                using (StreamReader sr = File.OpenText(MAINFILEPATH + "_indexes.txt"))
                {
                    string s = "";
                    int i = 0;
                    while ((s = sr.ReadLine()) != null)
                    {
                        tables[i, 0] = s.Split("|")[0].Split(",")[0];
                        tables[i, 1] = s.Split("|")[0].Split(",")[1];
                        tables[i, 2] = s.Split("|")[0].Split(",")[2];
                        tables[i, 3] = s.Split("|")[1];
                        if (consoleWrite) Console.WriteLine(s.Split(",")[1]);                        
                        i++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write("When read indexes: ");
                Console.WriteLine(ex.ToString());
            }
            Console.WriteLine();
            return tables;
        }
    }
}
