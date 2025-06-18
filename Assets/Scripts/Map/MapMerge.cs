using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShipSide
{
    Bow,       
    Stern,      
    Port,       
    Starboard   
}

public class MapMerge : MonoBehaviour
{
    // Direction vectors for pointy-topped layout
    private static readonly Dictionary<ShipSide, Vector2Int> PointyOffsets = new Dictionary<ShipSide, Vector2Int>
    {
        { ShipSide.Bow, new Vector2Int(0, -1) },   // Up
        { ShipSide.Stern, new Vector2Int(0, 1) },    // Down
        { ShipSide.Port, new Vector2Int(-1, 0) },   // Left
        { ShipSide.Starboard, new Vector2Int(1, 0) }     // Right
    };

    // Direction vectors for flat-topped layout
    private static readonly Dictionary<ShipSide, Vector2Int> FlatOffsets = new Dictionary<ShipSide, Vector2Int>
    {
        { ShipSide.Bow, new Vector2Int(1, 0) },    // Right
        { ShipSide.Stern, new Vector2Int(-1, 0) },   // Left
        { ShipSide.Port, new Vector2Int(0, -1) },   // Up
        { ShipSide.Starboard, new Vector2Int(0, 1) }     // Down
    };

    /// <summary>
    /// Calculate offsets for two boards when merged together. This function is
    /// pure so the math can be unit tested without any scene objects.
    /// </summary>
    public static (Vector2Int offsetA, Vector2Int offsetB, Vector2Int merged)
        GetMergeLayout(Vector2Int sizeA, Vector2Int sizeB, ShipSide side)
    {
        Vector2Int offsetA = Vector2Int.zero;
        Vector2Int offsetB = Vector2Int.zero;
        Vector2Int merged = Vector2Int.zero;

        switch (side)
        {
            case ShipSide.Bow:
                // Ship B sits above ship A
                offsetB = new Vector2Int(0, sizeA.y);
                merged = new Vector2Int(Mathf.Max(sizeA.x, sizeB.x), sizeA.y + sizeB.y);
                break;
            case ShipSide.Stern:
                // Ship B attaches below, so shift ship A up
                offsetA = new Vector2Int(0, sizeB.y);
                merged = new Vector2Int(Mathf.Max(sizeA.x, sizeB.x), sizeA.y + sizeB.y);
                break;
            case ShipSide.Port:
                // Ship B attaches on the left
                offsetA = new Vector2Int(sizeB.x, 0);
                merged = new Vector2Int(sizeA.x + sizeB.x, Mathf.Max(sizeA.y, sizeB.y));
                break;
            case ShipSide.Starboard:
                // Ship B attaches on the right
                offsetB = new Vector2Int(sizeA.x, 0);
                merged = new Vector2Int(sizeA.x + sizeB.x, Mathf.Max(sizeA.y, sizeB.y));
                break;
        }

        return (offsetA, offsetB, merged);
    }

    public static void MergeBoards(Map map, Board shipA, Board shipB, ShipSide sideToAttach)
    {
        var offsets = map.isFlatTopped ? FlatOffsets : PointyOffsets;
        Vector2Int dir = offsets[sideToAttach];

        int widthA = shipA._size_X;
        int heightA = shipA._size_Y;
        int widthB = shipB._size_X;
        int heightB = shipB._size_Y;

        // Determine where each board should be positioned in the merged grid.
        var layout = GetMergeLayout(new Vector2Int(widthA, heightA), new Vector2Int(widthB, heightB), sideToAttach);

        int offsetXA = layout.offsetA.x;
        int offsetYA = layout.offsetA.y;
        int offsetXB = layout.offsetB.x;
        int offsetYB = layout.offsetB.y;
        int mergedWidth = layout.merged.x;
        int mergedHeight = layout.merged.y;

        int minQ = int.MaxValue;
        int minR = int.MaxValue;

        // First pass to compute cube coordinate bounds
        for (int x = 0; x < widthA; x++)
        {
            for (int y = 0; y < heightA; y++)
            {
                Tile tile = shipA.get_Tile(x, y);
                if (tile != null)
                {
                    int mx = x + offsetXA;
                    int my = y + offsetYA;
                    Vector3Int cube = HexUtils.OffsetToCube(new Vector2Int(mx, my), map.isFlatTopped);
                    if (cube.x < minQ) minQ = cube.x;
                    if (cube.y < minR) minR = cube.y;
                }
            }
        }

        for (int x = 0; x < widthB; x++)
        {
            for (int y = 0; y < heightB; y++)
            {
                Tile tile = shipB.get_Tile(x, y);
                if (tile != null)
                {
                    int mx = x + offsetXB;
                    int my = y + offsetYB;
                    Vector3Int cube = HexUtils.OffsetToCube(new Vector2Int(mx, my), map.isFlatTopped);
                    if (cube.x < minQ) minQ = cube.x;
                    if (cube.y < minR) minR = cube.y;
                }
            }
        }

        map.PlayArea = new Board(new Vector2Int(mergedWidth, mergedHeight), -minQ, -minR);

        // Gather Ship A tiles
        for (int x = 0; x < widthA; x++)
        {
            for (int y = 0; y < heightA; y++)
            {
                Tile tile = shipA.get_Tile(x, y);
                if (tile != null)
                {
                    int mx = x + offsetXA;
                    int my = y + offsetYA;
                    Vector2Int tilePos = new Vector2Int(mx, my);

                    // Boards are currently generated using even-q/even-r layout
                    // so we pass 'useOdd:false' explicitly for clarity.
                    Vector3Int cubeCoords = HexUtils.OffsetToCube(tilePos, map.isFlatTopped, false);

                    Tile newTile = Object.Instantiate(tile, map.transform);
                    newTile.SetPosition(tilePos);
                    newTile.SetQUSPosition(cubeCoords.x, cubeCoords.y);
                    newTile.SetPawnPos();
                    newTile.SetupHexRenderer(map.innerSize, map.outerSize, map.isFlatTopped);

                    map.PlayArea.set_Tile(mx, my, newTile);
                    newTile.transform.position = map.GetHexPositionFromCoordinate(tilePos);

                }
            }
        }

        // Gather Ship B tiles
        for (int x = 0; x < widthB; x++)
        {
            for (int y = 0; y < heightB; y++)
            {
                Tile tile = shipB.get_Tile(x, y);
                if (tile != null)
                {
                    int mx = x + offsetXB;
                    int my = y + offsetYB;
                    Vector2Int tilePos = new Vector2Int(mx, my);

                    Vector3Int cubeCoords = HexUtils.OffsetToCube(tilePos, map.isFlatTopped, false);

                    Tile newTile = Object.Instantiate(tile, map.transform);
                    newTile.SetPosition(tilePos);
                    newTile.SetQUSPosition(cubeCoords.x, cubeCoords.y);
                    newTile.SetPawnPos();
                    newTile.SetupHexRenderer(map.innerSize, map.outerSize, map.isFlatTopped);

                    map.PlayArea.set_Tile(mx, my, newTile);
                    newTile.transform.position = map.GetHexPositionFromCoordinate(tilePos);
                }
            }
        }

        FillNulls(map.PlayArea, map);
        map.MapSize = new Vector2Int(mergedWidth, mergedHeight);
        map.SetNeighbours(map.PlayArea, map.isFlatTopped);
        map.setFirstHex();
        Debug.Log("Merged boards into map.");
    }

    public static void FillNulls(Board board, Map mapData)
    {
        int sizeX = board._size_X;
        int sizeY = board._size_Y;

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                Tile existing = board.get_Tile(x, y);
                if (existing == null)
                {
                    // Convert offset to cube
                    Vector2Int offsetCoords = new Vector2Int(x, y);
                    Vector3Int cubeCoords = HexUtils.OffsetToCube(offsetCoords, mapData.isFlatTopped, false);

                    // Create empty GameObject with Tile component
                    GameObject holder = new GameObject($"Hex {x},{y}", typeof(Tile));
                    Tile tile = holder.GetComponent<Tile>();
                    tile.Data = mapData.TileTypes[0];

                    // Setup position and height
                    int height = tile.Data == mapData.TileTypes[0] ? 5 : 20;
                    tile.SetPositionAndHeight(offsetCoords, cubeCoords.x, cubeCoords.y, height);

                    // Set world position
                    Vector3 worldPos = mapData.GetHexPositionFromCoordinate(offsetCoords);
                    worldPos.y += tile.Height / 2f;
                    holder.transform.position = worldPos;

                    // Parent to map
                    holder.transform.SetParent(mapData.transform);

                    // Instantiate mesh
                    GameObject visual = Object.Instantiate(tile.Data.TilePrefab, holder.transform);
                    visual.transform.position += new Vector3(0, tile.Height / 2f - 1f, 0);

                    // Setup hex visuals
                    tile.SetupHexRenderer(mapData.innerSize, mapData.outerSize, mapData.isFlatTopped);

                    // Final setup
                    tile.SetPosition(offsetCoords);
                    tile.SetQUSPosition(cubeCoords.x, cubeCoords.y);
                    tile.SetPawnPos();

                    // Assign to board
                    board.set_Tile(x, y, tile);
                }
            }
        }
    }


}

