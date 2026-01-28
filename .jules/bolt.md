## 2024-05-23 - [O(N) Graph Initialization in Pathfinding]
**Learning:** The pathfinding system was re-initializing the entire grid graph (creating `PathNode` objects and calculating heuristics for ALL tiles) for *every* path request. This is O(N) where N is map size, which is extremely wasteful for short paths or frequent queries (like mouse movement highlighting).
**Action:** Implemented lazy node initialization and a Binary Heap (MinHeap) for the open list. Always check if a system processes the entire dataset when it only needs a subset.

## 2024-10-27 - [Material Instantiation Overhead]
**Learning:** Accessing the `.material` property of a Unity `MeshRenderer` automatically creates a unique instance (clone) of the material if one doesn't exist, which can lead to significant memory and CPU overhead if done frequently (e.g., on every tile of a large hex map).
**Action:** Replaced `.material` with `.sharedMaterial` for read-only access in `HexRenderer.currentMat()`. Always use `.sharedMaterial` when you do not intend to modify the material properties for that specific instance.
