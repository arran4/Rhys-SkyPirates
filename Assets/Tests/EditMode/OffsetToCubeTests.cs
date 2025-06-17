using NUnit.Framework;
using UnityEngine;

// Quick checks for HexUtils.OffsetToCube.
// The ASCII diagrams in the method's XML comment illustrate both layouts:
//   Flat topped (odd-q)
//     (col,row)
//      0,0   1,0   2,0
//        0,1   1,1   2,1
//      0,2   1,2   2,2
//   Pointy topped (odd-r)
//     (col,row)
//     0,0  1,0  2,0
//     0,1  1,1  2,1
//     0,2  1,2  2,2
// These tests verify a few coordinates from those diagrams to ensure the
// implementation matches the documented formulas.
public class OffsetToCubeTests
{
    [Test]
    public void OffsetToCube_FlatTopped()
    {
        Vector2Int[] offsets =
        {
            new Vector2Int(0, 0),
            new Vector2Int(1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(1, 1),
            new Vector2Int(2, 2)
        };

        Vector3Int[] expected =
        {
            new Vector3Int(0, 0, 0),
            new Vector3Int(1, -1, 0),
            new Vector3Int(0, 1, -1),
            new Vector3Int(1, 0, -1),
            new Vector3Int(2, 1, -3)
        };

        for (int i = 0; i < offsets.Length; i++)
        {
            Vector3Int cube = HexUtils.OffsetToCube(offsets[i], true);
            Assert.AreEqual(expected[i], cube, $"flat-topped {offsets[i]}");
        }
    }

    [Test]
    public void OffsetToCube_PointyTopped()
    {
        Vector2Int[] offsets =
        {
            new Vector2Int(0, 0),
            new Vector2Int(1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(1, 1),
            new Vector2Int(2, 2)
        };

        Vector3Int[] expected =
        {
            new Vector3Int(0, 0, 0),
            new Vector3Int(1, 0, -1),
            new Vector3Int(-1, 1, 0),
            new Vector3Int(0, 1, -1),
            new Vector3Int(1, 2, -3)
        };

        for (int i = 0; i < offsets.Length; i++)
        {
            Vector3Int cube = HexUtils.OffsetToCube(offsets[i], false);
            Assert.AreEqual(expected[i], cube, $"pointy-topped {offsets[i]}");
        }
    }
}
