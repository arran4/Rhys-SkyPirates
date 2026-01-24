## 2024-05-23 - [Unity Allocations in Hot Paths]
**Learning:** The `FindPath` method was allocating new collections (Dictionary, List) and the caller was creating arrays (`ToArray()`) on every mouse movement event. This caused significant GC pressure.
**Action:** When working on Unity interaction scripts (like highlighting), always check for repeated allocations in `Update` or event handlers. Cache data structures where possible, especially for static or semi-static data like grid maps.
