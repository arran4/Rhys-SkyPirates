using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HexRenderer))]
[RequireComponent(typeof(MeshCollider))]

public class Tile : MonoBehaviour
{
    public int column { get; private set; }
    public int row { get; private set; }
    public HexRenderer Hex;
    public MeshCollider HexColider;
    public void Awake()
    {
        Hex = GetComponent<HexRenderer>();
        HexColider = GetComponent<MeshCollider>();
        HexColider.sharedMesh = Hex.H_Mesh;
    }

    private void Update()
    {
        SetMesh();
    }

    public void SetMesh()
    {
        
        HexColider.convex = false;
    }
}
