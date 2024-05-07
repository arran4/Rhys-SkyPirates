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
    }

    private void Update()
    {
        SetMesh();
    }

    //this is a tempory solution to a rediculous issue. will need to figure out something more substantual.
    public void SetMesh()
    {
        HexColider.sharedMesh = Hex.H_ColiderMesh;
        HexColider.convex = false;
    }
}
