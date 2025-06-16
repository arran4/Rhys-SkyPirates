using NUnit.Framework;
using UnityEngine;

/*
 * These tests validate the new qOffset/rOffset logic and dictionary based cube lookup.
 *
 * Offsets shift the cube origin to the center of the board. For a 3x3 board the
 * layout looks like:
 *
 *   (-1,1)  (0,1)  (1,1)
 *   (-1,0)  (0,0)  (1,0)
 *   (-1,-1) (0,-1) (1,-1)
 *
 * Cube (0,0,0) corresponds to board index (1,1) when offsets are (1,1).
 */
public class BoardOffsetTests
{
    private Tile CreateTile(Vector2Int pos, int q, int r)
    {
        GameObject go = new GameObject();
        Tile tile = go.AddComponent<Tile>();
        tile.Data = ScriptableObject.CreateInstance<TileDataSO>();
        tile.SetPositionAndHeight(pos, q, r, 0);
        return tile;
    }

    [Test]
    public void SearchTileByCube_UsesOffsets()
    {
        Board board = new Board(new Vector2Int(3,3)); // qOffset/rOffset default to 1
        Tile center = CreateTile(new Vector2Int(1,1), 0,0);
        board.set_Tile(1,1,center);
        Tile looked = board.SearchTileByCubeCoordinates(0,0,0);
        Assert.AreEqual(center, looked);
    }

    [Test]
    public void GetTileByCube_ReturnsFromDictionary()
    {
        Board board = new Board(new Vector2Int(3,3));
        Tile tile = CreateTile(new Vector2Int(2,0), 1,-1);
        board.set_Tile(2,0,tile);
        Vector3Int cube = new Vector3Int(1,-1,0);
        Assert.AreEqual(tile, board.GetTileByCube(cube));
    }
}
