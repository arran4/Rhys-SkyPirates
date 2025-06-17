using UnityEngine;

public static class HexUtils
{
    /// <summary>
    /// Convert offset coordinates <paramref name="offset"/> (column, row)
    /// to cube coordinates using the standard axial conversions from
    /// <see href="https://www.redblobgames.com/grids/hex-grids/">Red Blob Games</see>.
    ///
    /// When <paramref name="isFlatTopped"/> is <c>true</c> the method assumes an
    /// <b>even-q</b> layout (columns shifted down when the column index is even).
    /// When <paramref name="isFlatTopped"/> is <c>false</c> an <b>even-r</b>
    /// layout is used (rows shifted right on even indices).
    /// Set <paramref name="useOdd"/> to convert from the odd-q/odd-r variants.
    /// </summary>
    public static Vector3Int OffsetToCube(Vector2Int offset, bool isFlatTopped,
                                          bool useOdd = false)
    {
        int col = offset.x;
        int row = offset.y;

        if (isFlatTopped)
        {
            // Flat topped hexes use column based (q) offsets
            int q = col;
            int r = useOdd
                ? row - (col - (col & 1)) / 2  // odd-q
                : row - (col + (col & 1)) / 2; // even-q
            int s = -q - r;
            return new Vector3Int(q, r, s);
        }
        else
        {
            // Pointy topped hexes use row based (r) offsets
            int q = useOdd
                ? col - (row - (row & 1)) / 2   // odd-r
                : col - (row + (row & 1)) / 2;  // even-r
            int r = row;
            int s = -q - r;
            return new Vector3Int(q, r, s);
        }
    }

    // 6 Hex Directions in Cube Coordinates
    public static readonly Vector3Int[] CubeDirections = new Vector3Int[]
    {
        new Vector3Int(1, -1, 0),
        new Vector3Int(1, 0, -1),
        new Vector3Int(0, 1, -1),
        new Vector3Int(-1, 1, 0),
        new Vector3Int(-1, 0, 1),
        new Vector3Int(0, -1, 1)
    };
}
