using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools.Utils;

/*
 * These tests verify the vertex positions returned by
 * HexRenderer.GetPoint for both hex orientations. Unity draws
 * hexagonal meshes differently depending on whether the flat side or
 * the point is at the top. The renderer stores this as the
 * `isFlatTopped` flag. By calling GetPoint directly we avoid needing
 * to spawn a real mesh and can simply check the calculated Vector3
 * values. The expected coordinates are derived from basic hexagon
 * geometry where a hex with size 1 has corner offsets of `sqrt(3)/2`
 * on the z-axis when flat topped and the x-axis when pointy topped.
 */
// Tests follow the style of other EditMode tests in this folder so
// they can be executed via Unity's Test Runner.
public class HexRendererGetPointTests
{
    // Helper subclass to expose the protected GetPoint method
    private class TestHexRenderer : HexRenderer
    {
        public Vector3 CallGetPoint(float size, float height, int index)
        {
            return base.GetPoint(size, height, index);
        }
    }

    private TestHexRenderer CreateRenderer(bool flat)
    {
        var go = new GameObject("HexRendererTest");
        var renderer = go.AddComponent<TestHexRenderer>();
        renderer.isFlatTopped = flat;
        return renderer;
    }

    [Test]
    public void GetPoint_FlatTopped_ReturnsExpectedVertices()
    {
        var renderer = CreateRenderer(true);
        float size = 1f;
        float height = 0f;
        float s = Mathf.Sqrt(3f) / 2f;
        Vector3[] expected = {
            // Corners starting at the right and going counter-clockwise.
            // With a flat top the offsets alternate along the z-axis.
            new Vector3(1f, height, 0f),
            new Vector3(0.5f, height, s),
            new Vector3(-0.5f, height, s),
            new Vector3(-1f, height, 0f),
            new Vector3(-0.5f, height, -s),
            new Vector3(0.5f, height, -s)
        };

        for (int i = 0; i < 6; i++)
        {
            Vector3 actual = renderer.CallGetPoint(size, height, i);
            Assert.That(actual, Is.EqualTo(expected[i]).Using(Vector3ComparerWithEqualsOperator.Instance));
        }
        Object.DestroyImmediate(renderer.gameObject);
    }

    [Test]
    public void GetPoint_PointyTopped_ReturnsExpectedVertices()
    {
        var renderer = CreateRenderer(false);
        float size = 1f;
        float height = 0f;
        float s = Mathf.Sqrt(3f) / 2f;
        Vector3[] expected = {
            // For pointy-topped hexes the pattern rotates so the x offsets
            // use sqrt(3)/2 while the z values alternate by 0.5.
            new Vector3(s, height, -0.5f),
            new Vector3(s, height, 0.5f),
            new Vector3(0f, height, 1f),
            new Vector3(-s, height, 0.5f),
            new Vector3(-s, height, -0.5f),
            new Vector3(0f, height, -1f)
        };

        for (int i = 0; i < 6; i++)
        {
            Vector3 actual = renderer.CallGetPoint(size, height, i);
            Assert.That(actual, Is.EqualTo(expected[i]).Using(Vector3ComparerWithEqualsOperator.Instance));
        }
        Object.DestroyImmediate(renderer.gameObject);
    }

    [Test]
    public void GetPoint_HeightParameterAffectsYCoordinate_Flat()
    {
        var renderer = CreateRenderer(true);
        Vector3 result = renderer.CallGetPoint(1f, 2f, 0);
        Assert.AreEqual(2f, result.y);
        Object.DestroyImmediate(renderer.gameObject);
    }

    [Test]
    public void GetPoint_HeightParameterAffectsYCoordinate_Pointy()
    {
        var renderer = CreateRenderer(false);
        Vector3 result = renderer.CallGetPoint(1f, -1.5f, 3);
        Assert.AreEqual(-1.5f, result.y);
        Object.DestroyImmediate(renderer.gameObject);
    }
}
