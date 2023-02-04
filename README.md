# Advanced-B-Plus-Tree

In my project, I have configured the B+ tree algorithm using fewer nodes than its standard design. This design requires less data transfer between nodes, thus providing advantages in terms of capacity and speed. In addition, thanks to this design, which requires less data transfer between nodes, search, insertion and deletion operations can be performed faster.

## **Insertion Rules**
1. Find correct leafnode and try to give it
2. Give to right
3. Give to left
4. Split

.

### Starting node, an empty node, both leafnode and root

<img src="https://user-images.githubusercontent.com/68849018/216774418-2e9d92c2-8c19-436b-b98d-5f2e6d9425b5.png" alt="drawing" style="width:300px;"/>

### Insert "**1**"

<img src="https://user-images.githubusercontent.com/68849018/216774496-827de561-6a72-4045-9031-d44a024203ff.png" alt="drawing" style="width:300px;"/>

### Insert "**2, 3 and 4**"

<img src="https://user-images.githubusercontent.com/68849018/216774497-1adfb4cc-de9c-4859-bedb-24109b1de8c3.png" alt="drawing" style="width:300px;"/>

### Try to Insert "**5**" but not enough free space

<img src="https://user-images.githubusercontent.com/68849018/216774499-b41b3e04-d521-4825-8ab4-8228ac0862d5.png" alt="drawing" style="width:300px;"/>

### Split the node into 2, and give "**5**" to right leafnode, create parent and bind them

<img src="https://user-images.githubusercontent.com/68849018/216774500-14937bad-bda4-4039-bdf7-e151279ef3cc.png" alt="drawing" style="width:250px;"/>

### Set parent node's value

<img src="https://user-images.githubusercontent.com/68849018/216774501-1455969c-cc2d-4964-bcc9-9bd3fe3c2c3f.png" alt="drawing" style="width:250px;"/>

### Insert "**6**"

<img src="https://user-images.githubusercontent.com/68849018/216774503-90a9d6d8-d2ef-4f09-a11c-c18c50cc0440.png" alt="drawing" style="width:250px;"/>

### Try Insert "**7**", shift 3 to left sibling

<img src="https://user-images.githubusercontent.com/68849018/216774505-e6e12775-c821-46ca-82f3-67d48a75dc6c.png" alt="drawing" style="width:250px;"/>

### Insert "**7**"

<img src="https://user-images.githubusercontent.com/68849018/216774506-a4493e09-3b5e-4d6a-b8eb-91d23dbc2d70.png" alt="drawing" style="width:250px;"/>

### Set parent node's value

<img src="https://user-images.githubusercontent.com/68849018/216774509-c3f01496-c700-426a-a7b4-c540b216d404.png" alt="drawing" style="width:250px;"/>

### Try Insert "**8**", shift 4 to left sibling

<img src="https://user-images.githubusercontent.com/68849018/216774510-dbdc340a-5433-494d-a4cf-f4f7f79dcbeb.png" alt="drawing" style="width:250px;"/>

### Insert "**8**" and Set parent node's value

<img src="https://user-images.githubusercontent.com/68849018/216774511-f7074438-9f3c-42f4-84cb-ef2cf19ab7c5.png" alt="drawing" style="width:250px;"/>

### Try Insert "**9**" but there is no space to shift

<img src="https://user-images.githubusercontent.com/68849018/216774435-253bae95-8401-476b-b37b-341ea33efb77.png" alt="drawing" style="width:250px;"/>

### Split leafnode and give "**9**" to right leafnode,

<img src="https://user-images.githubusercontent.com/68849018/216774437-1501e7c3-ef04-4dbe-92ab-b2f0b8c14e36.png" alt="drawing" style="width:250px;"/>

### Set parent node's value

<img src="https://user-images.githubusercontent.com/68849018/216774438-f74d2924-af0b-4b30-bfd6-f6849c81f320.png" alt="drawing" style="width:250px;"/>

### Insert "**10, 11 and 12**" (with shift values to left sibling and reset parent node's value)

<img src="https://user-images.githubusercontent.com/68849018/216774439-349fcff6-80c9-447d-b293-f412b8548d64.png" alt="drawing" style="width:250px;"/>

### Try Insert "**13**", Split leafnode 

<img src="https://user-images.githubusercontent.com/68849018/216774440-b934d852-2695-4718-8cab-c0761fd1a638.png" alt="drawing" 
style="width:250px;"/>

### Insert "14, 15 and 16"

<img src="https://user-images.githubusercontent.com/68849018/216774444-c03e8ee8-2688-43f7-8266-4199e7da1181.png" alt="drawing" style="width:250px;"/>

### Insert "17, 18, 19 and 20", our single parent node is full now

<img src="https://user-images.githubusercontent.com/68849018/216774459-31877477-fd00-4d35-a7b0-731478a038ac.png" alt="drawing" style="width:300px;"/>

### Try Insert "**21**"

<img src="https://user-images.githubusercontent.com/68849018/216774476-c499d9b8-0dc4-460c-8c8e-7d79e9e88083.png" alt="drawing" style="width:350px;"/>

### Split leafnode and give "**21**" to right leafnode, but old leafnode's parent is full

<img src="https://user-images.githubusercontent.com/68849018/216774483-891d7a4c-1f1c-415f-b184-2aa92b4b3cad.png" alt="drawing" style="width:350px;"/>

### Split the parent node into 2, share child nodes equally, create a new parent node, reset all parents values
Initial value of parent nodes that do not point to a leaf node is calculated as:
1. Find the largest data value of the first set of nodes it points to
2. Set the next largest value in the whole tree

<img src="https://user-images.githubusercontent.com/68849018/216774488-b98db99c-f3a2-4275-ba8c-5b5d199c2c2f.png" alt="drawing" style="width:400px;"/>

### Insert values from 22 to 33 in the same way

<img src="https://user-images.githubusercontent.com/68849018/216774491-e7c6f91b-f55c-4bfc-b3e8-9ad2007ce432.png" alt="drawing" style="width:400px;"/>

### The extra leafnode is requested to be binded to the right, the leafnodes are shifted and the leftmost leafnode is given to the left sibling

<img src="https://user-images.githubusercontent.com/68849018/216774492-af7d1fa4-871c-48cf-a9c1-f5f21e2d9951.png" 
alt="drawing" style="width:450px;"/>

### The values of all parent nodes are reset in the same way

<img src="https://user-images.githubusercontent.com/68849018/216774494-5454a2e2-f827-4934-bc6b-8f75af2db1ca.png" alt="drawing" style="width:450px;"/>

### Final form with 100 data Inserted

<img src="https://user-images.githubusercontent.com/68849018/216774495-b2f6d0f3-ea0d-44d6-ada9-c2fa2fcd3637.png" alt="drawing" style="width:1000px;"/>

