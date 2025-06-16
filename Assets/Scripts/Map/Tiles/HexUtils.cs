using UnityEngine;

public static class HexUtils
{
    /// <summary>
    /// Converts column/row <c>offset</c> coordinates to cube <c>(q,r,s)</c>.
    /// <para>
    /// Flat topped layout (<c>odd-q</c>) shifts odd columns down:
    /// <code>
    /// (col,row)
    ///  0,0   1,0   2,0
    ///    0,1   1,1   2,1
    ///  0,2   1,2   2,2
    /// q = col
    /// r = row - (col + (col &amp; 1)) / 2
    /// s = -q - r
    /// </code>
    /// Pointy topped layout (<c>odd-r</c>) shifts odd rows right:
    /// <code>
    /// (col,row)
    /// 0,0  1,0  2,0
    /// 0,1  1,1  2,1
    /// 0,2  1,2  2,2
    /// q = col - (row + (row &amp; 1)) / 2
    /// r = row
    /// s = -q - r
    /// </code>
    /// </para>
    /// </summary>
    public static Vector3Int OffsetToCube(Vector2Int offset, bool isFlatTopped)
    {
        int col = offset.x;
        int row = offset.y;

        if (isFlatTopped)
        {
            int q = col;
            int r = row - (col + (col & 1)) / 2;
            int s = -q - r;
            return new Vector3Int(q, r, s);
        }
        else
        {
            int q = col - (row + (row & 1)) / 2;
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
