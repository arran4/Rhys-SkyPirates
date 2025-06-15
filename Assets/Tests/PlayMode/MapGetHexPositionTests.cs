using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools.Utils;

public class MapGetHexPositionTests
{
    private Map CreateMap(float innerSize, float outerSize, bool isFlatTopped)
    {
        var go = new GameObject();
        var map = go.AddComponent<Map>();
        map.innerSize = innerSize;
        map.outerSize = outerSize;
        map.isFlatTopped = isFlatTopped;
        return map;
    }

    /*
        These tests walk through the math used by Map.GetHexPositionFromCoordinate.
        The grid uses axial coordinates (q,r).  Below is a small diagram of the
        coordinate system used in the tests:

               (-1,1)  (0,1)  (1,1)
                  \      |      /
               (-1,0) (0,0) (1,0)
                  /      |      \
               (-1,-1)(0,-1)(1,-1)

        width  = sqrt(3) * size
        height = 2 * size

        For flat topped hexes the origin is shifted right by width/2.
        Each test below shows the manual calculation for a given (q,r) pair.
    */
    [Test]
    public void FlatTopped_GetHexPosition_ReturnsExpectedValues()
    {
        // Arrange: create a map using axial coordinates with known sizes
        var map = CreateMap(innerSize:0.5f, outerSize:1f, isFlatTopped:true);

        // Manual calculation for q=0,r=0 when flat topped:
        // width = sqrt(3)*size = 1.73205
        // height = 2*size = 2
        // offset = width/2 = 0.866025
        // x = q*(width*0.9)+offset = 0+0.866025
        // y = -((r + q/2)*height*0.9) = -0
        Assert.That(map.GetHexPositionFromCoordinate(new Vector2Int(0,0)),
            Is.EqualTo(new Vector3(0.866025f,0f,0f)).Using(Vector3ComparerWithEqualsOperator.Instance));

        // Small table of expected positions (size=1, gap=0.9):
        // | q | r | (x,z)         |
        // | 0 | 0 | (0.866, 0.000)|
        // | 1 | 0 | (2.425,-0.900)|
        // | 0 | 1 | (0.866,-1.800)|

        // q=1,r=0 -> x=1*(1.73205*0.9)+0.866025=2.42487, y=-(0.5*2*0.9)=-0.9
        Vector3 expected1 = new Vector3(2.42487f, 0f, -0.9f);
        Vector3 actual1 = map.GetHexPositionFromCoordinate(new Vector2Int(1,0));
        Assert.That(actual1, Is.EqualTo(expected1).Using(Vector3ComparerWithEqualsOperator.Instance));

        // q=0,r=1 -> x=0+0.866025=0.866025, y=-(1*2*0.9)=-1.8
        Vector3 expected2 = new Vector3(0.866025f, 0f, -1.8f);
        Vector3 actual2 = map.GetHexPositionFromCoordinate(new Vector2Int(0,1));
        Assert.That(actual2, Is.EqualTo(expected2).Using(Vector3ComparerWithEqualsOperator.Instance));
    }

    // Negative coordinates should work too. This helps ensure that the formula
    // does not rely on clamped values.
    [Test]
    public void FlatTopped_NegativeCoordinates_AreCalculatedCorrectly()
    {
        var map = CreateMap(innerSize:0.5f, outerSize:1f, isFlatTopped:true);

        // q=-1, r=0 yields x=-0.69282 and y=0.9 using the same math as above
        Vector3 expected = new Vector3(-0.69282f, 0f, 0.9f);
        Vector3 actual = map.GetHexPositionFromCoordinate(new Vector2Int(-1,0));
        Assert.That(actual, Is.EqualTo(expected).Using(Vector3ComparerWithEqualsOperator.Instance));
    }

    // The same checks for a pointy topped layout. Here the origin is exactly at
    // (0,0,0) which can make grid math easier to reason about.
    [Test]
    public void PointyTopped_GetHexPosition_ReturnsExpectedValues()
    {
        var map = CreateMap(innerSize:0.5f, outerSize:1f, isFlatTopped:false);

        // For pointy topped offset=0
        // q=0,r=0 -> (0,0,0)
        Assert.That(map.GetHexPositionFromCoordinate(new Vector2Int(0,0)),
            Is.EqualTo(Vector3.zero).Using(Vector3ComparerWithEqualsOperator.Instance));

        // Small table of expected positions (size=1, gap=0.9):
        // | q | r | (x,z)         |
        // | 1 | 0 | (1.559,-0.900)|
        // | 1 | 1 | (1.559,-2.700)|

        // q=1,r=0 -> x=1*(1.73205*0.9)=1.55885, y=-(0.5*2*0.9)=-0.9
        Vector3 expected1 = new Vector3(1.55885f,0f,-0.9f);
        Vector3 actual1 = map.GetHexPositionFromCoordinate(new Vector2Int(1,0));
        Assert.That(actual1, Is.EqualTo(expected1).Using(Vector3ComparerWithEqualsOperator.Instance));

        // q=1,r=1 -> x=1.55885, y=-(1.5*2*0.9)=-2.7
        Vector3 expected2 = new Vector3(1.55885f,0f,-2.7f);
        Vector3 actual2 = map.GetHexPositionFromCoordinate(new Vector2Int(1,1));
        Assert.That(actual2, Is.EqualTo(expected2).Using(Vector3ComparerWithEqualsOperator.Instance));

    }

    // Example of a regression style test highlighting an apparent bug. For a
    // flat topped map you might expect the coordinate (0,0) to sit at the
    // world origin, but the current implementation applies an additional offset
    // which shifts everything to the right. This test intentionally expects the
    // origin to be at Vector3.zero and therefore fails until the bug is fixed.
    [Test]
    public void FlatTopped_Origin_ShouldBeAtZero_BugExample()
    {
        var map = CreateMap(innerSize:0.5f, outerSize:1f, isFlatTopped:true);

        // Fails: actual value is (0.866025, 0, 0). Once you remove the offset
        // when q and r are both zero this assertion will pass and you can delete
        // this test.
        Assert.That(
            map.GetHexPositionFromCoordinate(new Vector2Int(0, 0)),
            Is.EqualTo(Vector3.zero).Using(Vector3ComparerWithEqualsOperator.Instance));
    }
}
