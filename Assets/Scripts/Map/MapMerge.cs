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

    public static void MergeBoards(Map map, Board shipA, Board shipB, ShipSide sideToAttach)
    {
        bool pointyTopped = !map.isFlatTopped;
        var offsets = pointyTopped ? PointyOffsets : FlatOffsets;
        Vector2Int dir = offsets[sideToAttach];

        int widthA = shipA._size_X;
        int heightA = shipA._size_Y;
        int widthB = shipB._size_X;
        int heightB = shipB._size_Y;

        int offsetXA = 0; // Offset for Ship A
        int offsetYA = 0;
        int offsetXB = 0; // Offset for Ship B
        int offsetYB = 0;

        int mergedWidth = 0;
        int mergedHeight = 0;

        switch (sideToAttach)
        {
            case ShipSide.Bow:
                // Attach ship B above ship A
                offsetXB = 0;
                offsetYB = heightA;
                mergedWidth = Mathf.Max(widthA, widthB);
                mergedHeight = heightA + heightB;
                break;

            case ShipSide.Stern:
                // Attach ship B below ship A — shift ship A up by heightB
                offsetXA = 0;
                offsetYA = heightB;  // Shift ship A up
                offsetXB = 0;
                offsetYB = 0;       // Ship B at bottom (0,0)
                mergedWidth = Mathf.Max(widthA, widthB);
                mergedHeight = heightA + heightB;
                break;

            case ShipSide.Port:
                // Attach ship B to left of ship A — shift ship A right by widthB
                offsetXA = widthB;   // Shift ship A right
                offsetYA = 0;
                offsetXB = 0;
                offsetYB = 0;       // Ship B at left (0,0)
                mergedWidth = widthA + widthB;
                mergedHeight = Mathf.Max(heightA, heightB);
                break;

            case ShipSide.Starboard:
                // Attach ship B to right of ship A
                offsetXA = 0;
                offsetYA = 0;
                offsetXB = widthA;
                offsetYB = 0;
                mergedWidth = widthA + widthB;
                mergedHeight = Mathf.Max(heightA, heightB);
                break;
        }

        map.PlayArea = new Board(new Vector2Int(mergedWidth, mergedHeight));

        // Copy Ship A tiles with offset
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
                    Vector3Int cubeCoords = HexUtils.OffsetToCube(tilePos, map.isFlatTopped);

                    Tile newTile = Object.Instantiate(tile, map.transform);
                    newTile.SetPosition(tilePos);
                    newTile.SetQUSPosition(cubeCoords.x, cubeCoords.y);
                    newTile.SetPawnPos();

                    map.PlayArea.set_Tile(mx, my, newTile);
                    newTile.transform.position = map.GetHexPositionFromCoordinate(tilePos);
                }
            }
        }

        // Copy Ship B tiles with offset
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
                    Vector3Int cubeCoords = HexUtils.OffsetToCube(tilePos, map.isFlatTopped);

                    Tile newTile = Object.Instantiate(tile, map.transform);
                    newTile.SetPosition(tilePos);
                    newTile.SetQUSPosition(cubeCoords.x, cubeCoords.y);
                    newTile.SetPawnPos();

                    map.PlayArea.set_Tile(mx, my, newTile);
                    newTile.transform.position = map.GetHexPositionFromCoordinate(tilePos);
                }
            }
        }

        map.MapSize = new Vector2Int(mergedWidth, mergedHeight);

        // Set neighbors and first hex
        map.SetNeighbours();
        map.setFirstHex();

        Debug.Log("Merged boards into map.");
    }

}

