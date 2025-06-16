using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Unit tests for <see cref="Pathfinding"/>.
///
/// A small 3x3 hex board is created for the scenarios below. Axial
/// coordinates increase to the right (q) and upward (r):
///
/// <code>
/// (0,2)-(1,2)-(2,2)
///   |     |     |
/// (0,1)-(1,1)-(2,1)
///   |     |     |
/// (0,0)-(1,0)-(2,0)
/// </code>
///
/// The first test expects a straight path from (0,0) to (2,0). The
/// second marks the interior tiles impassable and verifies that no
/// route can be found.
/// </summary>
public class PathfindingTests
{
    private Board CreateBoard(int size, int defaultCost = 1)
    {
        Board board = new Board(new Vector2Int(size, size));
        Map map = new GameObject("TestMap").AddComponent<Map>();
        map.isFlatTopped = false;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                GameObject go = new GameObject($"Tile_{x}_{y}");
                Tile tile = go.AddComponent<Tile>();
                tile.Data = ScriptableObject.CreateInstance<TileDataSO>();
                tile.Data.MovementCost = defaultCost;
                tile.SetPositionAndHeight(new Vector2Int(x, y), x - size / 2, y - size / 2, 0);
                board.set_Tile(x, y, tile);
            }
        }

        map.SetNeighbours(board, map.isFlatTopped);
        return board;
    }

    [Test]
    public void FindPath_ReturnsExpectedRoute()
    {
        Board board = CreateBoard(3);
        Tile start = board.get_Tile(0, 0);
        Tile end = board.get_Tile(2, 0);

        Pathfinding pf = new Pathfinding();
        Tile[] allTiles = board.GetAllTiles().ToArray();
        List<Vector3Int> path = pf.FindPath(start, end, allTiles);

        List<Vector3Int> expected = new List<Vector3Int>
        {
            start.ReturnSquareCoOrds(),
            board.get_Tile(1, 0).ReturnSquareCoOrds(),
            end.ReturnSquareCoOrds()
        };

        CollectionAssert.AreEqual(expected, path);
    }

    [Test]
    public void FindPath_NoRoute_ReturnsEmpty()
    {
        Board board = CreateBoard(3);
        Tile start = board.get_Tile(0, 0);
        Tile end = board.get_Tile(2, 2);

        foreach (Tile t in board.GetAllTiles())
        {
            if (t != start && t != end)
            {
                t.Data.MovementCost = 0; // make tile unwalkable
            }
        }

        Pathfinding pf = new Pathfinding();
        Tile[] allTiles = board.GetAllTiles().ToArray();
        List<Vector3Int> path = pf.FindPath(start, end, allTiles);

        Assert.IsEmpty(path);
    }
}
