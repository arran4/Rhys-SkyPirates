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
    public float height { get; private set; }

    public TileDataSO Data;

    public List<Tile> Neighbours;

    public Material BaseMat;

    [SerializeField]
    private int s { get { return -column - row; } }


    public void Awake()
    {
        Hex = GetComponent<HexRenderer>();
        HexColider = GetComponent<MeshCollider>();
    }

    public void Start()
    {
        Hex.DrawMesh();
        Hex.GetColliderMesh();
        SetMesh();
    }

    //this is a tempory solution to a rediculous issue. will need to figure out something more substantual.
    public void SetMesh()
    {
        HexColider.sharedMesh = Hex.H_ColiderMesh;
        HexColider.convex = false;
    }

    public void setPositon(Vector2Int coords)
    {
        column = coords.x;
        row = coords.y;
    }

    public void setHeight(int  Height)
    {
        height = Height;
    }
    public Tile CheckNeighbours(Vector2 Direction)
    {
        //Can seperate out the camera direction calculation to somewhere else removing 15~ lines here.
        Tile Closest = null; 
        float minDist = Mathf.Infinity;
        Vector3 Forawrd = new Vector3();
        CameraController Cam = Camera.main.GetComponentInParent<CameraController>();    
        if (Direction.y > 0)
        {
            Forawrd = (Cam.Forward * Hex.outerSize) + this.transform.position;
        }
        if(Direction.y < 0)
        {
            Forawrd = (-Cam.Forward * Hex.outerSize) + this.transform.position;        
        }
        if(Direction.x > 0)
        {
            Forawrd = (Cam.Right * Hex.outerSize) + this.transform.position;
        }
        if (Direction.x < 0)
        {
            Forawrd = (-Cam.Right * Hex.outerSize) + this.transform.position;         
        }
        foreach (Tile Next in Neighbours)
        {
            float dist = Vector3.Distance(Next.transform.position, Forawrd);
            if (dist < minDist)
            {
                Closest = Next;
                minDist = dist;
            }
        }
        return Closest;
    }
}
