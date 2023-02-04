# Advanced-B-Plus-Tree


Insertion Rules
1. Find correct leafnode and try to give it
2. Give to right
3. Give to left
4. Split

.

1. Starting node, an empty node, both leafnode and root

<img src="[drawing.jpg](https://user-images.githubusercontent.com/68849018/216774418-2e9d92c2-8c19-436b-b98d-5f2e6d9425b5.png)" alt="drawing" style="width:200px;"/>

![1 drawio](https://user-images.githubusercontent.com/68849018/216774418-2e9d92c2-8c19-436b-b98d-5f2e6d9425b5.png  | width=100)

2. Insert "**1**"

![1 drawio (1)](https://user-images.githubusercontent.com/68849018/216774496-827de561-6a72-4045-9031-d44a024203ff.png)

3. Insert "**2, 3 and 4**"

![1 drawio (2)](https://user-images.githubusercontent.com/68849018/216774497-1adfb4cc-de9c-4859-bedb-24109b1de8c3.png)

4. Try to Insert "**5**" but not enough free space

![1 drawio (3)](https://user-images.githubusercontent.com/68849018/216774499-b41b3e04-d521-4825-8ab4-8228ac0862d5.png)

5. Split the node into 2, and give "**5**" to right leafnode, create parent and bind them

![1 drawio (4)](https://user-images.githubusercontent.com/68849018/216774500-14937bad-bda4-4039-bdf7-e151279ef3cc.png)

6. Set parent node's value

![1 drawio (5)](https://user-images.githubusercontent.com/68849018/216774501-1455969c-cc2d-4964-bcc9-9bd3fe3c2c3f.png)

![1 drawio (6)](https://user-images.githubusercontent.com/68849018/216774503-90a9d6d8-d2ef-4f09-a11c-c18c50cc0440.png)

![1 drawio (7)](https://user-images.githubusercontent.com/68849018/216774505-e6e12775-c821-46ca-82f3-67d48a75dc6c.png)

![1 drawio (8)](https://user-images.githubusercontent.com/68849018/216774506-a4493e09-3b5e-4d6a-b8eb-91d23dbc2d70.png)

![1 drawio (9)](https://user-images.githubusercontent.com/68849018/216774509-c3f01496-c700-426a-a7b4-c540b216d404.png)

![1 drawio (10)](https://user-images.githubusercontent.com/68849018/216774510-dbdc340a-5433-494d-a4cf-f4f7f79dcbeb.png)

![1 drawio (11)](https://user-images.githubusercontent.com/68849018/216774511-f7074438-9f3c-42f4-84cb-ef2cf19ab7c5.png)

![1 drawio (12)](https://user-images.githubusercontent.com/68849018/216774435-253bae95-8401-476b-b37b-341ea33efb77.png)

![1 drawio (13)](https://user-images.githubusercontent.com/68849018/216774437-1501e7c3-ef04-4dbe-92ab-b2f0b8c14e36.png)

![1 drawio (14)](https://user-images.githubusercontent.com/68849018/216774438-f74d2924-af0b-4b30-bfd6-f6849c81f320.png)

![1 drawio (15)](https://user-images.githubusercontent.com/68849018/216774439-349fcff6-80c9-447d-b293-f412b8548d64.png)

![1 drawio (16)](https://user-images.githubusercontent.com/68849018/216774440-b934d852-2695-4718-8cab-c0761fd1a638.png)

![1 drawio (17)](https://user-images.githubusercontent.com/68849018/216774444-c03e8ee8-2688-43f7-8266-4199e7da1181.png)

![1 drawio (18)](https://user-images.githubusercontent.com/68849018/216774459-31877477-fd00-4d35-a7b0-731478a038ac.png)

![1 drawio (19)](https://user-images.githubusercontent.com/68849018/216774476-c499d9b8-0dc4-460c-8c8e-7d79e9e88083.png)

![1 drawio (20)](https://user-images.githubusercontent.com/68849018/216774483-891d7a4c-1f1c-415f-b184-2aa92b4b3cad.png)

![1 drawio (23)](https://user-images.githubusercontent.com/68849018/216774488-b98db99c-f3a2-4275-ba8c-5b5d199c2c2f.png)

![1 drawio (24)](https://user-images.githubusercontent.com/68849018/216774491-e7c6f91b-f55c-4bfc-b3e8-9ad2007ce432.png)

![1 drawio (25)](https://user-images.githubusercontent.com/68849018/216774492-af7d1fa4-871c-48cf-a9c1-f5f21e2d9951.png)

![1 drawio (26)](https://user-images.githubusercontent.com/68849018/216774493-7de4b8b7-0ab7-4c77-b882-e9de6a8b0fb5.png)

![1 drawio (27)](https://user-images.githubusercontent.com/68849018/216774494-5454a2e2-f827-4934-bc6b-8f75af2db1ca.png)

![1 drawio (28)](https://user-images.githubusercontent.com/68849018/216774495-b2f6d0f3-ea0d-44d6-ada9-c2fa2fcd3637.png)

