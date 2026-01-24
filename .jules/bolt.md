## 2024-05-23 - [O(N) Graph Initialization in Pathfinding]
**Learning:** The pathfinding system was re-initializing the entire grid graph (creating `PathNode` objects and calculating heuristics for ALL tiles) for *every* path request. This is O(N) where N is map size, which is extremely wasteful for short paths or frequent queries (like mouse movement highlighting).
**Action:** Implemented lazy node initialization and a Binary Heap (MinHeap) for the open list. Always check if a system processes the entire dataset when it only needs a subset.
