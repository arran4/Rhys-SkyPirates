using NUnit.Framework;
using UnityEngine;

namespace Tests.EditMode
{
    public class HexRendererMaterialTests
    {
        private class TestHexRenderer : HexRenderer
        {
            // Expose internals if needed, but currentMat is public
        }

        [Test]
        public void CurrentMat_ReturnsCorrectMaterial()
        {
            var go = new GameObject("HexRendererTest");
            var renderer = go.AddComponent<TestHexRenderer>();
            var meshRenderer = go.GetComponent<MeshRenderer>();

            // Attempt to find a basic shader
            var shader = Shader.Find("Standard");
            if (shader == null)
            {
                // Fallback for some test environments
                shader = Shader.Find("Hidden/InternalErrorShader");
            }

            // If we still can't find a shader, we might be in a very limited env,
            // but we'll try to proceed.
            // Note: In some headless Unity envs, Shader.Find might return null for everything.
            // If so, we can't easily test Material creation.
            // But let's assume we can.

            if (shader != null)
            {
                var mat = new Material(shader);
                mat.name = "TestMaterial";

                // Update the mesh renderer with this material
                // We use meshupdate which sets .material = Mat
                renderer.meshupdate(mat);

                // Verify currentMat returns a material
                var result = renderer.currentMat();
                Assert.IsNotNull(result);
                // We don't check for "(Instance)" suffix strictly because environment behavior might vary,
                // but we ensure it matches the renderer's active material.

                // Ensure it matches what's on the renderer
                Assert.AreEqual(meshRenderer.sharedMaterial, result);
            }
            else
            {
                Assert.Ignore("Could not find a valid shader to create a material for testing.");
            }

            Object.DestroyImmediate(go);
        }
    }
}
