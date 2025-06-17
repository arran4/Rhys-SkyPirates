// Tests demonstrating that Board correctly handles custom axial offsets.
// ---------------------------------------------------------------------
// Edit Mode tests can be run from Unity's Test Runner (Window > General > Test Runner).
// These checks focus on the pure logic so no scene setup is required.

using NUnit.Framework;
using UnityEngine;

public class BoardOffsetTests
{
    // Board.SearchTileByCubeCoordinates should translate cube coordinates using
    // the offsets supplied to the constructor. Here we place a single tile at
    // board index (0,0) which corresponds to cube (-1,-1,2) when the offsets are
    // (1,1).
    [Test]
    public void SearchTile_UsesOffsets()
    {
        Board board = new Board(new Vector2Int(2, 2), 1, 1);
        var go = new GameObject("tile");
        var tile = go.AddComponent<Tile>();
        tile.Data = ScriptableObject.CreateInstance<TileDataSO>();
        tile.SetPositionAndHeight(new Vector2Int(0, 0), -1, -1, 1f);
        board.set_Tile(0, 0, tile);

        // Lookup by cube coordinates should return the same tile instance.
        Tile found = board.SearchTileByCubeCoordinates(-1, -1, 2);
        Assert.AreSame(tile, found);
    }

    // Rotating a board should compute new offsets based on the minimum cube
    // coordinates of the rotated tiles. A 2x2 board rotated 60° clockwise has
    // cube bounds q=[-2,0], r=[0,1] resulting in offsets (2,0).
    [Test]
    public void RotateBoard_ComputesNewOffsets()
    {
        Board board = new Board(new Vector2Int(2, 2), 1, 1);
        for (int x = 0; x < 2; x++)
        {
            for (int y = 0; y < 2; y++)
            {
                var go = new GameObject($"t_{x}_{y}");
                var tile = go.AddComponent<Tile>();
                tile.Data = ScriptableObject.CreateInstance<TileDataSO>();
                int q = x - 1;
                int r = y - 1;
                tile.SetPositionAndHeight(new Vector2Int(x, y), q, r, 1f);
                board.set_Tile(x, y, tile);
            }
        }

        Board rotated = BoardRotator.RotateBoard(board, BoardRotator.Rotation.Rotate60CW);

        Assert.AreEqual(2, rotated.qOffset);
        Assert.AreEqual(0, rotated.rOffset);
        Assert.AreEqual(3, rotated._size_X);
        Assert.AreEqual(2, rotated._size_Y);
    }
}
