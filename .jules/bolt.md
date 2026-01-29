## 2024-05-23 - [O(N) Graph Initialization in Pathfinding]
**Learning:** The pathfinding system was re-initializing the entire grid graph (creating `PathNode` objects and calculating heuristics for ALL tiles) for *every* path request. This is O(N) where N is map size, which is extremely wasteful for short paths or frequent queries (like mouse movement highlighting).
**Action:** Implemented lazy node initialization and a Binary Heap (MinHeap) for the open list. Always check if a system processes the entire dataset when it only needs a subset.

## Material Instantiation Optimization
- **Issue:** Accessing or assigning the `.material` property on a `MeshRenderer` implicitly creates a material clone (instance), causing unnecessary allocation and preventing batching.
- **Solution:** Use `.sharedMaterial` for both reading and assigning materials when the intention is to use the shared asset. This prevents cloning.
- **Verification:** Verified by code inspection and creation of a benchmark script (`Assets/Tests/HexRendererPerformance.cs`) to measure the impact of repeated material assignment.
