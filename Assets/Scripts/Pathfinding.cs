using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    private const int MOVE_COST = 10;

    public List<Vector3Int> FindPath(Tile startTile, Tile endTile, Tile[] allTiles)
    {
        // Optimization: Use HashSet for O(1) lookup of valid tiles.
        // This avoids creating PathNodes for the entire map upfront.
        HashSet<Tile> validTiles = new HashSet<Tile>(allTiles);

        if (!validTiles.Contains(startTile) || !validTiles.Contains(endTile))
        {
            return new List<Vector3Int>();
        }

        // Dictionary to store path nodes that we have visited or are considering (Lazy initialization)
        Dictionary<Tile, PathNode> nodes = new Dictionary<Tile, PathNode>();

        PathNode GetPathNode(Tile tile)
        {
            if (!nodes.TryGetValue(tile, out PathNode node))
            {
                node = new PathNode
                {
                    Tile = tile,
                    GCost = int.MaxValue,
                    HCost = CalculateHexDistance(tile, endTile),
                    IsWalkable = tile.Data.MovementCost > 0,
                    CameFrom = null
                };
                nodes[tile] = node;
            }
            return node;
        }

        PathNode startNode = GetPathNode(startTile);
        startNode.GCost = 0;

        MinHeap<PathNode> openList = new MinHeap<PathNode>(allTiles.Length);
        openList.Add(startNode);

        HashSet<PathNode> closedList = new HashSet<PathNode>();

        while (openList.Count > 0)
        {
            PathNode currentNode = openList.RemoveFirst();

            if (currentNode.Tile == endTile)
            {
                return CalculatePath(startNode, currentNode);
            }

            closedList.Add(currentNode);

            foreach (Tile neighbor in currentNode.Tile.Neighbours)
            {
                if (neighbor == null || !validTiles.Contains(neighbor)) continue;

                PathNode neighborNode = GetPathNode(neighbor);
                if (closedList.Contains(neighborNode) || !neighborNode.IsWalkable) continue;

                int tentativeGCost = currentNode.GCost + MOVE_COST;
                if (tentativeGCost < neighborNode.GCost)
                {
                    neighborNode.CameFrom = currentNode;
                    neighborNode.GCost = tentativeGCost;

                    if (!openList.Contains(neighborNode))
                    {
                        openList.Add(neighborNode);
                    }
                    else
                    {
                        openList.UpdateItem(neighborNode);
                    }
                }
            }
        }

        return new List<Vector3Int>();
    }

    private List<Vector3Int> CalculatePath(PathNode startNode, PathNode endNode)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        PathNode currentNode = endNode;
        while (currentNode != startNode)
        {
            path.Add(new Vector3Int(currentNode.Tile.QAxis, currentNode.Tile.RAxis, currentNode.Tile.SAxis));
            currentNode = currentNode.CameFrom;
        }
        path.Add(new Vector3Int(startNode.Tile.QAxis, startNode.Tile.RAxis, startNode.Tile.SAxis));
        path.Reverse();
        return path;
    }

    private int CalculateHexDistance(Tile a, Tile b)
    {
        return (Mathf.Abs(a.QAxis - b.QAxis) + Mathf.Abs(a.RAxis - b.RAxis) + Mathf.Abs(a.SAxis - b.SAxis)) / 2;
    }

    private class PathNode : IHeapItem<PathNode>
    {
        public Tile Tile;
        public int GCost;
        public int HCost;
        public int FCost => GCost + HCost;
        public bool IsWalkable;
        public PathNode CameFrom;
        public int HeapIndex { get; set; }

        public int CompareTo(PathNode other)
        {
            int compare = FCost.CompareTo(other.FCost);
            if (compare == 0)
            {
                compare = HCost.CompareTo(other.HCost);
            }
            return compare;
        }
    }

    private interface IHeapItem<T> : System.IComparable<T>
    {
        int HeapIndex { get; set; }
    }

    private class MinHeap<T> where T : IHeapItem<T>
    {
        T[] items;
        int currentItemCount;

        public MinHeap(int maxHeapSize)
        {
            items = new T[maxHeapSize];
        }

        public void Add(T item)
        {
            item.HeapIndex = currentItemCount;
            items[currentItemCount] = item;
            SortUp(item);
            currentItemCount++;
        }

        public T RemoveFirst()
        {
            T firstItem = items[0];
            currentItemCount--;
            items[0] = items[currentItemCount];
            items[0].HeapIndex = 0;
            SortDown(items[0]);
            return firstItem;
        }

        public void UpdateItem(T item)
        {
            SortUp(item);
        }

        public int Count => currentItemCount;

        public bool Contains(T item)
        {
            return item.HeapIndex < currentItemCount && Equals(items[item.HeapIndex], item);
        }

        void SortDown(T item)
        {
            while (true)
            {
                int childIndexLeft = item.HeapIndex * 2 + 1;
                int childIndexRight = item.HeapIndex * 2 + 2;
                int swapIndex = 0;

                if (childIndexLeft < currentItemCount)
                {
                    swapIndex = childIndexLeft;

                    if (childIndexRight < currentItemCount)
                    {
                        if (items[childIndexLeft].CompareTo(items[childIndexRight]) > 0)
                        {
                            swapIndex = childIndexRight;
                        }
                    }

                    if (item.CompareTo(items[swapIndex]) > 0)
                    {
                        Swap(item, items[swapIndex]);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
        }

        void SortUp(T item)
        {
            int parentIndex = (item.HeapIndex - 1) / 2;
            while (true)
            {
                T parentItem = items[parentIndex];
                if (item.CompareTo(parentItem) < 0)
                {
                    Swap(item, parentItem);
                }
                else
                {
                    break;
                }

                parentIndex = (item.HeapIndex - 1) / 2;
            }
        }

        void Swap(T itemA, T itemB)
        {
            items[itemA.HeapIndex] = itemB;
            items[itemB.HeapIndex] = itemA;
            int itemAIndex = itemA.HeapIndex;
            itemA.HeapIndex = itemB.HeapIndex;
            itemB.HeapIndex = itemAIndex;
        }
    }
}

public struct PathfinderSelections
{
    public List<List<Vector3Int>> Paths; // List of paths, where each path is a list of Vector3Ints
    public int NumSelections; // Number of selections or paths stored

    public PathfinderSelections(List<List<Vector3Int>> paths, int numSelections)
    {
        Paths = paths;
        NumSelections = numSelections;
    }

    public void AddPath(List<Vector3Int> path)
    {
        if (Paths == null)
            Paths = new List<List<Vector3Int>>();

        Paths.Add(path);
        NumSelections = Paths.Count;
    }

    public void ClearPaths()
    {
        if (Paths != null)
            Paths.Clear();

        NumSelections = 0;
    }

    public void ClearLastPath()
    {
        if (Paths.Count != 0)
        {
            Paths.RemoveAt(Paths.Count - 1);
            NumSelections = Paths.Count;
        }
    }
    public int TotalPathLength()
    {
        if (Paths == null || Paths.Count == 0) return 0;

        int total = 0;
        foreach (var path in Paths)
        {
            total += path.Count;
        }

        // Subtract 1 for each path (each includes its start tile)
        return total - Paths.Count;
    }
}
