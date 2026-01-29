using UnityEngine;
using System.Diagnostics;

public class HexRendererPerformance : MonoBehaviour
{
    public HexRenderer targetRenderer;
    public Material matA;
    public Material matB;
    public int iterations = 10000;

    void Start()
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<HexRenderer>();
        }

        if (targetRenderer == null || matA == null || matB == null)
        {
            UnityEngine.Debug.LogError("Assign Target Renderer, Mat A and Mat B");
            return;
        }

        RunBenchmark();
    }

    void RunBenchmark()
    {
        Stopwatch sw = new Stopwatch();

        // Warmup
        for (int i = 0; i < 100; i++)
        {
            targetRenderer.meshupdate(i % 2 == 0 ? matA : matB);
        }

        sw.Start();
        for (int i = 0; i < iterations; i++)
        {
            targetRenderer.meshupdate(i % 2 == 0 ? matA : matB);
        }
        sw.Stop();

        UnityEngine.Debug.Log($"Benchmark finished. Iterations: {iterations}. Time: {sw.ElapsedMilliseconds} ms.");
    }
}
