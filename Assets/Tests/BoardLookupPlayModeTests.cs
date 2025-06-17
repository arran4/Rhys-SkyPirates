// PlayMode tests verifying board lookup consistency.
// --------------------------------------------------
// Open Unity's Test Runner (Window > General > Test Runner) and run the
// PlayMode suite to execute these tests.
//
// Each test generates a board through a different code path (random
// generation, save/load, merging, rotation) and then checks that every
// tile can be retrieved using its cube coordinates.  These lookups are
// fundamental for gameplay and should remain constant time.

using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BoardLookupPlayModeTests
{
    /// <summary>
    /// Creates a bare bones <see cref="Map"/> with two simple tile types.
    /// These tests do not rely on any real game data, so we generate
    /// minimal scriptable objects at runtime.
    /// </summary>
    private Map CreateBasicMap(Vector2Int size)
    {
        var go = new GameObject("Map");
        var map = go.AddComponent<Map>();
        map.MapSize = size;
        map.innerSize = 0.5f;
        map.outerSize = 1f;
        map.isFlatTopped = true;
        map.TileTypes = new List<TileDataSO>();

        // Create two dummy tile types so the board can instantiate tiles.
        var tile1 = ScriptableObject.CreateInstance<TileDataSO>();
        tile1.UniqueID = "type1";
        tile1.TilePrefab = new GameObject("prefab1");
        tile1.BaseMat = new Material(Shader.Find("Standard"));
        map.TileTypes.Add(tile1);

        var tile2 = ScriptableObject.CreateInstance<TileDataSO>();
        tile2.UniqueID = "type2";
        tile2.TilePrefab = new GameObject("prefab2");
        tile2.BaseMat = tile1.BaseMat;
        map.TileTypes.Add(tile2);

        return map;
    }

    /// <summary>
    /// Many of the generation utilities rely on a scene containing a
    /// Camera with a <see cref="CameraController"/>.  PlayMode tests start
    /// with an empty scene, so we spawn one if needed.
    /// </summary>
    private void EnsureCamera()
    {
        if (Camera.main == null)
        {
            var camGO = new GameObject("Main Camera");
            camGO.tag = "MainCamera";
            camGO.AddComponent<Camera>();
            camGO.AddComponent<CameraController>();
        }
    }

    /// <summary>
    /// Helper used by each test to verify that searching the board by cube
    /// coordinates returns the exact same tile instance that we started with.
    /// </summary>
    private void AssertBoardLookups(Board board)
    {
        foreach (var tile in board.GetAllTiles())
        {
            Vector3Int cube = new Vector3Int(tile.QAxis, tile.RAxis, tile.SAxis);
            Assert.AreSame(tile, board.SearchTileByCubeCoordinates(cube.x, cube.y, cube.z));
            Assert.AreSame(tile, board.GetTileByCube(cube));
        }
    }

    /// <summary>
    /// Checks that cube coordinates begin at (-size/2,-size/2) so the board is
    /// centred around the origin.  This mirrors how boards are generated in
    /// RandomGeneration and other utilities.
    /// </summary>
    private void AssertCenteredCoords(Board board)
    {
        int qStart = -board._size_X / 2;
        int rStart = -board._size_Y / 2;

        Assert.AreEqual(qStart, board.get_Tile(0, 0).QAxis);
        Assert.AreEqual(rStart, board.get_Tile(0, 0).RAxis);

        Assert.AreEqual(qStart + board._size_X - 1,
            board.get_Tile(board._size_X - 1, board._size_Y - 1).QAxis);
        Assert.AreEqual(rStart + board._size_Y - 1,
            board.get_Tile(board._size_X - 1, board._size_Y - 1).RAxis);
    }

    /// <summary>
    /// Verifies tiles can be found after randomly generating a board.
    /// </summary>
    [UnityTest]
    public IEnumerator RandomGenerationBoardLookup()
    {
        // Arrange - create a simple map and generator
        EnsureCamera();
        var map = CreateBasicMap(new Vector2Int(3, 3));
        var genGO = new GameObject("Gen");
        var gen = genGO.AddComponent<RandomGeneration>();

        // Act - generate a random board
        Board board = gen.Generate(map);
        yield return null; // wait a frame for any coroutines

        // Assert - every tile must be reachable via cube coordinates
        AssertBoardLookups(board);

        Object.Destroy(genGO);
        Object.Destroy(map.gameObject);
    }

    /// <summary>
    /// Saves a generated board to JSON and then loads it back to ensure the
    /// lookup structures are preserved in the serialized data.
    /// </summary>
    [UnityTest]
    public IEnumerator LoadBoardFromJsonLookup()
    {
        // Arrange - create a map then save it to disk
        EnsureCamera();
        var map = CreateBasicMap(new Vector2Int(2, 2));
        var genGO = new GameObject("Gen");
        var gen = genGO.AddComponent<RandomGeneration>();
        map.PlayArea = gen.Generate(map);
        var saveGO = new GameObject("SaveLoad");
        var slm = saveGO.AddComponent<SaveLoadManager>();
        yield return null; // allow components to initialise

        string path = Path.Combine(Application.persistentDataPath, "temp_board.json");
        slm.SaveMapToJson(map, path);

        // Act - load a board back from the saved file
        Board loaded = SaveLoadManager.LoadBoardFromJson(path, map, map.transform);
        yield return null;

        // Assert
        AssertBoardLookups(loaded);
        AssertCenteredCoords(loaded);

        Object.Destroy(genGO);
        Object.Destroy(saveGO);
        Object.Destroy(map.gameObject);
    }

    /// <summary>
    /// Merges two boards together and checks that the resulting board can
    /// still look up tiles correctly.
    /// </summary>
    [UnityTest]
    public IEnumerator MergeBoardsLookup()
    {
        // Arrange - build two boards we can merge
        EnsureCamera();
        var map = CreateBasicMap(new Vector2Int(2, 2));
        var genGO = new GameObject("Gen");
        var gen = genGO.AddComponent<RandomGeneration>();
        Board a = gen.Generate(map);
        Board b = gen.Generate(map);

        // Act - merge the boards together
        MapMerge.MergeBoards(map, a, b, ShipSide.Bow);
        yield return null;

        // Assert
        AssertBoardLookups(map.PlayArea);

        Object.Destroy(genGO);
        Object.Destroy(map.gameObject);
    }

    /// <summary>
    /// Merging should initialise each tile's renderer using the map's
    /// orientation so that meshes face the correct direction.
    /// </summary>
    [UnityTest]
    public IEnumerator MergeBoards_RespectsMapOrientation([Values(true, false)] bool flat)
    {
        // Arrange
        EnsureCamera();
        var map = CreateBasicMap(new Vector2Int(2, 2));
        map.isFlatTopped = flat;
        var genGO = new GameObject("Gen");
        var gen = genGO.AddComponent<RandomGeneration>();
        Board a = gen.Generate(map);
        Board b = gen.Generate(map);

        // Act
        MapMerge.MergeBoards(map, a, b, ShipSide.Bow);
        yield return null;

        // Assert - every tile should adopt the map's orientation
        foreach (var tile in map.PlayArea.GetAllTiles())
        {
            Assert.AreEqual(flat, tile.Hex.isFlatTopped);
        }

        Object.Destroy(genGO);
        Object.Destroy(map.gameObject);
    }

    /// <summary>
    /// Rotates an existing board and confirms that rotated tiles maintain
    /// valid cube coordinate lookups.
    /// </summary>
    [UnityTest]
    public IEnumerator RotateBoardLookup()
    {
        // Arrange - generate a board we can rotate
        EnsureCamera();
        var map = CreateBasicMap(new Vector2Int(3, 3));
        var genGO = new GameObject("Gen");
        var gen = genGO.AddComponent<RandomGeneration>();
        Board board = gen.Generate(map);

        // Act - rotate the board 60 degrees clockwise
        Board rotated = BoardRotator.RotateBoard(board, BoardRotator.Rotation.Rotate60CW);
        yield return null;

        // Assert
        AssertBoardLookups(rotated);

        Object.Destroy(genGO);
        Object.Destroy(map.gameObject);
    }
}
