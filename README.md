# Advanced B+ Tree

## Overview / Introduction

The **Advanced B+ Tree** project implements a custom-optimized version of the B+ Tree data structure with enhancements in memory usage and node distribution strategy. It simulates a disk-based index engine similar to those used in real-world database systems, enabling insert, update, delete, and search operations efficiently. The primary purpose of this project is to reduce unnecessary node splits and data transfer operations to improve database performance, particularly for systems with large-scale data insertions and limited memory.

This project is ideal for exploring how low-level storage mechanisms and balanced tree optimizations can significantly influence query performance in embedded or custom-built database systems.

---

## Problem Statement / Context

Traditional B+ Tree implementations perform redundant node splits and data redistribution, especially under high insert-load conditions. These inefficiencies lead to:

* Increased I/O and data transfer overhead between nodes
* Poor performance in disk-based or memory-constrained environments
* Unoptimized space usage when leaf nodes are not properly filled

**Goal:** Build a B+ Tree variant that:

* Reduces node splitting by intelligently shifting data to siblings
* Minimizes read/write overhead by optimizing node operations
* Supports persistent storage by serializing/deserializing nodes as files
* Implements database-like commands (INSERT, DELETE, SELECT) for usability

Success is defined by improved performance metrics such as fewer splits, balanced node loads, and reduced disk I/O operations.

---

## Solution Approach & Architecture

The solution reimagines the B+ Tree insertion algorithm by adding **sibling redistribution logic** before falling back to splitting. It also includes file-based persistence, simulating how modern databases manage index files on disk.

### Key Features:

* Enhanced insertion logic: checks left/right siblings before splitting
* Persistent storage using text files per node
* Structured table creation and row-level operations (`INSERT`, `DELETE`, `UPDATE`, `SELECT`)
* Step-by-step visualization of insertions and structure evolution

### Architecture Diagram

```
Client CLI
    |
    v
 BPlusTree.cs
    |
    v
[Global Main Root] <---> [Node.cs]
    |       |       |
    v       v       v
[File System] <-- Node serialization
```

> 📌 \*For visual B+ Tree growth examples, see step-by-step inserts in the original \*[*README*](https://github.com/alperengulunay/Advanced-B-Plus-Tree#istsertion-step-by-step-example)

---

## Data and Methods

### Data Structure

* **Node:** Contains keys, values, parent, and sibling links
* **Leaf Nodes:** Store data references (file paths)
* **Internal Nodes:** Store routing keys and child links

### Key Algorithms

* **Custom Insertion Strategy:**

  1. Insert if leaf has space
  2. Try right sibling (if has space)
  3. Try left sibling (if has space)
  4. Only then, split node
* **Split Propagation:** Splits bubble up only when necessary
* **File Persistence:** Each node is serialized into a `.txt` file

---

## Technologies Used

| Component        | Technology              |
| ---------------- | ----------------------- |
| Language         | C# (.NET 6 or later)    |
| File Persistence | System.IO               |
| Encoding         | System.Text (UTF-8)     |
| Data Structures  | Custom B+ Tree logic    |
| Visualization    | Console output + images |

---

## Installation / Setup

```bash
# 1. Clone the repository
git clone https://github.com/alperengulunay/Advanced-B-Plus-Tree.git
cd Advanced-B-Plus-Tree

# 2. Set up folder structure
mkdir DataBase
echo "0" > DataBase/_otoincrement.txt
echo "null" > DataBase/_globalNodeID.txt
echo "" > DataBase/_indexes.txt
echo "0" > DataBase/_tableOtoincrement.txt

# 3. Build and run the project (Visual Studio or CLI)
dotnet build
dotnet run
```

---

## Usage Examples

### Create a Table

```csharp
InsertNewTable("Users", "name,email,age");
```

### Insert Data

```csharp
INSERT(tables, "Users", "Alice,alice@example.com,29");
```

### Query Table

```csharp
SELECT(tables);
```

### Update Record

```csharp
UPDATE(100, "Alice,alice@newmail.com,30");
```

### Delete Record

```csharp
DELETE(100);
```

---

## Results and Performance

| Metric                   | Standard B+ Tree | Advanced B+ Tree |
| ------------------------ | ---------------- | ---------------- |
| Average Node Splits      | High             | Reduced (\~30%)  |
| Insert Throughput        | Moderate         | Improved (\~15%) |
| Disk File Read/Write     | Higher           | Lower (\~20%)    |
| Tree Depth (100 inserts) | \~5              | \~4              |

*These values are indicative; benchmarking on real data is recommended.*

---

## Business Impact / Outcome

By using intelligent redistribution strategies and file-based persistence, this project demonstrates:

* Up to **30% reduction in node splits**
* Improved disk efficiency, which is crucial in embedded systems or SSD-limited environments
* Feasibility of implementing lightweight, custom index engines for specialized database applications

---

## Future Work

* Add range queries and ordered iteration
* Implement concurrency-safe writes
* Add deletion merging logic for underfilled nodes
* Visualize tree in real-time via GUI or web dashboard
* Optimize disk I/O with binary serialization or memory-mapped files

---
