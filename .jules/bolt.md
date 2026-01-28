## 2024-05-23 - [O(N) Graph Initialization in Pathfinding]
**Learning:** The pathfinding system was re-initializing the entire grid graph (creating `PathNode` objects and calculating heuristics for ALL tiles) for *every* path request. This is O(N) where N is map size, which is extremely wasteful for short paths or frequent queries (like mouse movement highlighting).
**Action:** Implemented lazy node initialization and a Binary Heap (MinHeap) for the open list. Always check if a system processes the entire dataset when it only needs a subset.

## 2024-05-24 - [Cached WaitForSeconds in Typewriter Effect]
**Learning:** The typewriter effect coroutine was creating a new `WaitForSeconds` object for every character printed. Since `WaitForSeconds` is a class, this resulted in thousands of allocations per dialogue line, creating unnecessary GC pressure.
**Action:** Cached the `WaitForSeconds` instance before the loop if the wait duration is constant. This reduces allocations from O(N) (N = string length) to O(1) per dialogue segment.
