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

    public static Board MergeBoards(Board shipA, Board shipB, ShipSide sideToAttach, bool pointyTopped)
    {
        int widthA = shipA._size_X;
        int heightA = shipA._size_Y;
        int widthB = shipB._size_X;
        int heightB = shipB._size_Y;

        Dictionary<ShipSide, Vector2Int> offsets = pointyTopped ? PointyOffsets : FlatOffsets;
        Vector2Int direction = offsets[sideToAttach];

        int offsetX = direction.x * widthA;
        int offsetY = direction.y * heightA;

        int minX = Mathf.Min(0, offsetX);
        int minY = Mathf.Min(0, offsetY);
        int maxX = Mathf.Max(widthA, offsetX + widthB);
        int maxY = Mathf.Max(heightA, offsetY + heightB);

        int mergedWidth = maxX - minX;
        int mergedHeight = maxY - minY;

        Board merged = new Board(new Vector2Int(mergedWidth, mergedHeight));

        int offsetAX = -minX;
        int offsetAY = -minY;
        int offsetBX = offsetAX + offsetX;
        int offsetBY = offsetAY + offsetY;

        // Copy tiles from Ship A
        for (int x = 0; x < widthA; x++)
        {
            for (int y = 0; y < heightA; y++)
            {
                Tile tile = shipA.get_Tile(x, y);
                if (tile != null)
                {
                    Tile newTile = Instantiate(tile);
                    merged.set_Tile(x + offsetAX, y + offsetAY, newTile);
                }
            }
        }

        // Copy tiles from Ship B
        for (int x = 0; x < widthB; x++)
        {
            for (int y = 0; y < heightB; y++)
            {
                Tile tile = shipB.get_Tile(x, y);
                if (tile != null)
                {
                    Tile newTile = Instantiate(tile);
                    merged.set_Tile(x + offsetBX, y + offsetBY, newTile);
                }
            }
        }

        return merged;
    }
}
