using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HexRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class Tile : MonoBehaviour
{
    public int Column { get; private set; }
    public int Row { get; private set; }
    public float Height { get; private set; }
    public HexRenderer Hex { get; private set; }
    public MeshCollider HexCollider { get; private set; }
    public TileDataSO Data;
    public List<Tile> Neighbours;
    public Material BaseMaterial;

    private CameraController cameraController;

    private void Awake()
    {
        Hex = GetComponent<HexRenderer>();
        HexCollider = GetComponent<MeshCollider>();
        cameraController = Camera.main.GetComponentInParent<CameraController>();
    }

    private void Start()
    {
        Hex.DrawMesh();
        Hex.GetColliderMesh();
        SetColliderMesh();
    }

    public void SetColliderMesh()
    {
        HexCollider.sharedMesh = Hex.H_ColiderMesh;
        HexCollider.convex = false;
    }

    public void SetPosition(Vector2Int coords)
    {
        Column = coords.x;
        Row = coords.y;
    }

    public void SetHeight(float height)
    {
        Height = height;
    }

    public Tile CheckNeighbours(Vector2 direction)
    {
        Vector3 forward = CalculateForwardVector(direction);

        Tile closest = null;
        float minDist = Mathf.Infinity;

        foreach (Tile next in Neighbours)
        {
            float dist = Vector3.Distance(next.transform.position, forward);
            if (dist < minDist)
            {
                closest = next;
                minDist = dist;
            }
        }

        return closest;
    }

    private Vector3 CalculateForwardVector(Vector2 direction)
    {
        Vector3 forward = Vector3.zero;

        if (cameraController == null)
        {
            Debug.LogError("Camera Controller is missing!");
            return forward;
        }

        if (direction.y > 0)
        {
            forward = (cameraController.Forward * Hex.outerSize) + transform.position;
        }
        else if (direction.y < 0)
        {
            forward = (-cameraController.Forward * Hex.outerSize) + transform.position;
        }
        else if (direction.x > 0)
        {
            forward = (cameraController.Right * Hex.outerSize) + transform.position;
        }
        else if (direction.x < 0)
        {
            forward = (-cameraController.Right * Hex.outerSize) + transform.position;
        }

        return forward;
    }
}