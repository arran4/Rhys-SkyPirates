using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

// PlayMode tests verifying that tile lookups remain valid after generating
// boards through multiple code paths. These lookups power many gameplay
// systems, so we guard against regressions here.
//
// Each test builds a board using a different approach (random generation,
// loading from JSON, merging, rotation) and then iterates over every tile.
// We ensure both search methods return the same tile instance for a given
// cube coordinate. `GetTileByCube` is expected to use a dictionary lookup and
// therefore must not enumerate the tile list.
public class BoardLookupConsistencyTests
{
    /// <summary>
    /// Creates a minimal <see cref="Map"/> instance with a single dummy tile
    /// type so that boards can be generated without relying on project assets.
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

        var tile = ScriptableObject.CreateInstance<TileDataSO>();
        tile.UniqueID = "basic";
        tile.TilePrefab = new GameObject("prefab");
        tile.BaseMat = new Material(Shader.Find("Standard"));
        map.TileTypes.Add(tile);

        return map;
    }

    /// <summary>
    /// PlayMode tests start with an empty scene. Many map utilities expect a
    /// camera with a <see cref="CameraController"/> so we create one if none
    /// exists.
    /// </summary>
    private void EnsureCamera()
    {
        if (Camera.main == null)
        {
            var cam = new GameObject("Main Camera");
            cam.tag = "MainCamera";
            cam.AddComponent<Camera>();
            cam.AddComponent<CameraController>();
        }
    }

    /// <summary>
    /// Iterates every tile on a board and verifies both lookup methods return
    /// the same tile instance when queried with its cube coordinates. This
    /// protects the dictionary-based lookup from accidental changes.
    /// </summary>
    private void ValidateLookups(Board board)
    {
        foreach (var tile in board.GetAllTiles())
        {
            Vector3Int cube = new Vector3Int(tile.QAxis, tile.RAxis, tile.SAxis);
            Assert.AreSame(tile, board.SearchTileByCubeCoordinates(cube.x, cube.y, cube.z));
            Assert.AreSame(tile, board.GetTileByCube(cube));
        }
    }

    /// <summary>
    /// Verifies lookup tables on a board produced by <see cref="RandomGeneration"/>.
    /// </summary>
    [UnityTest]
    public IEnumerator RandomGenerationLookup()
    {
        EnsureCamera();
        var map = CreateBasicMap(new Vector2Int(3, 3));
        var gen = new GameObject("Gen").AddComponent<RandomGeneration>();
        Board board = gen.Generate(map);
        yield return null; // allow any coroutines to finish
        ValidateLookups(board);
        Object.Destroy(gen.gameObject);
        Object.Destroy(map.gameObject);
    }

    /// <summary>
    /// Saves a generated board to JSON and loads it back to confirm that the
    /// lookup structures survive serialization.
    /// </summary>
    [UnityTest]
    public IEnumerator LoadBoardFromJsonLookup()
    {
        EnsureCamera();
        var map = CreateBasicMap(new Vector2Int(2, 2));
        var gen = new GameObject("Gen").AddComponent<RandomGeneration>();
        map.PlayArea = gen.Generate(map);
        var slGO = new GameObject("SaveLoad");
        var slm = slGO.AddComponent<SaveLoadManager>();
        yield return null; // wait for init
        string path = Path.Combine(Application.persistentDataPath, "temp_board.json");
        slm.SaveMapToJson(map, path);
        Board loaded = SaveLoadManager.LoadBoardFromJson(path, map, map.transform);
        yield return null;
        ValidateLookups(loaded);
        Object.Destroy(gen.gameObject);
        Object.Destroy(slGO);
        Object.Destroy(map.gameObject);
    }

    /// <summary>
    /// Merges two boards and ensures the resulting board can still look up
    /// tiles by cube coordinates.
    /// </summary>
    [UnityTest]
    public IEnumerator MergeBoardsLookup()
    {
        EnsureCamera();
        var map = CreateBasicMap(new Vector2Int(2, 2));
        var gen = new GameObject("Gen").AddComponent<RandomGeneration>();
        Board a = gen.Generate(map);
        Board b = gen.Generate(map);
        MapMerge.MergeBoards(map, a, b, ShipSide.Bow);
        yield return null;
        ValidateLookups(map.PlayArea);
        Object.Destroy(gen.gameObject);
        Object.Destroy(map.gameObject);
    }

    /// <summary>
    /// Rotates a board and verifies the rotated instance retains valid
    /// tile lookups.
    /// </summary>
    [UnityTest]
    public IEnumerator RotateBoardLookup()
    {
        EnsureCamera();
        var map = CreateBasicMap(new Vector2Int(3, 3));
        var gen = new GameObject("Gen").AddComponent<RandomGeneration>();
        Board board = gen.Generate(map);
        Board rotated = BoardRotator.RotateBoard(board, BoardRotator.Rotation.Rotate60CW);
        yield return null;
        ValidateLookups(rotated);
        Object.Destroy(gen.gameObject);
        Object.Destroy(map.gameObject);
    }
}
