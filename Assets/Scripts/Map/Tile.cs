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
        Tile Closest = null;
        if (Direction.y > 0)
        {
            Vector3 Forawrd = (Camera.main.GetComponentInParent<CameraController>().Forward * Hex.outerSize) + this.transform.position;
            float minDist = Mathf.Infinity;
            foreach (Tile Next in Neighbours)
            {
                float dist = Vector3.Distance(Next.transform.position, Forawrd);
                if (dist < minDist)
                {
                    Closest = Next;
                    minDist = dist;
                }
            }
        }
        if(Direction.y < 0)
        {
            Vector3 Forawrd = (-Camera.main.GetComponentInParent<CameraController>().Forward * Hex.outerSize) + this.transform.position;        
            float minDist = Mathf.Infinity;
            foreach (Tile Next in Neighbours)
            {
                float dist = Vector3.Distance(Next.transform.position, Forawrd);
                if (dist < minDist)
                {
                    Closest = Next;
                    minDist = dist;
                }
            }
        }
        if(Direction.x > 0)
        {
            Vector3 Forawrd = (Camera.main.GetComponentInParent<CameraController>().Right * Hex.outerSize) + this.transform.position;
            float minDist = Mathf.Infinity;
            foreach (Tile Next in Neighbours)
            {
                float dist = Vector3.Distance(Next.transform.position, Forawrd);
                if (dist < minDist)
                {
                    Closest = Next;
                    minDist = dist;
                }
            }
        }
        if (Direction.x < 0)
        {
            Vector3 Forawrd = (-Camera.main.GetComponentInParent<CameraController>().Right * Hex.outerSize) + this.transform.position;
            float minDist = Mathf.Infinity;
            foreach (Tile Next in Neighbours)
            {
                float dist = Vector3.Distance(Next.transform.position, Forawrd);
                if (dist < minDist)
                {
                    Closest = Next;
                    minDist = dist;
                }
            }
        }

        return Closest;
    }
}
