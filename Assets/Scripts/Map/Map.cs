using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField]
    private Vector2Int MapSize;

    public float innerSize , outerSize, height;
    public bool isFlatTopped;



    private Board PlayArea;

    public Material Mat;
    // Start is called before the first frame update
    void Start()
    {
        PlayArea = new Board(MapSize);

        for (int x = 0; x < MapSize.x; x++)
        {
            for (int y = 0; y < MapSize.y; y++)
            {
                GameObject Holder = new GameObject($"Hex {x},{y}", typeof(Tile));
                Holder.transform.position = GetHexPositionFromCoordinate(new Vector2Int(x, y));
                Tile ToAdd = Holder.GetComponent<Tile>();
                ToAdd.BaseMat = Mat;
                ToAdd.Hex.H_Mat = Mat;
                ToAdd.Hex.innerSize = innerSize;
                ToAdd.Hex.outerSize = outerSize;
                ToAdd.setHeight(Random.Range(1, 7) * 5);
                ToAdd.Hex.height = ToAdd.height;
                ToAdd.Hex.isFlatTopped = isFlatTopped;
                ToAdd.Hex.meshupdate(Mat);
                Holder.transform.position = new Vector3(Holder.transform.position.x, ToAdd.height / 2f, Holder.transform.position.z);
                ToAdd.transform.SetParent(this.transform);
                ToAdd.setPositon(new Vector2Int(x, y));
                PlayArea.set_Tile(x, y, ToAdd);

            }
        }

        for (int x = 0; x < MapSize.x; x++)
        {
            for (int y = 0; y < MapSize.y; y++)
            {
                Tile NeighbourGet = PlayArea.get_Tile(x, y);
                NeighbourGet.Neighbours = PlayArea.GetNeighbours(new Vector2Int(NeighbourGet.column, NeighbourGet.row));
            }
        }
        setFirstHex();
    }

    public Vector3 GetHexPositionFromCoordinate(Vector2Int Coordinates)
    {
        int column = Coordinates.x;
        int row = Coordinates.y;
        float width, height, xposition, yposition, horizontalDistance, VerticalDistance, offset;
        float size = outerSize;
        bool shouldOffset;

        if(!isFlatTopped)
        {
            shouldOffset = (row % 2) == 0;
            width = Mathf.Sqrt(3) * size;
            height = 2f * size;

            horizontalDistance = width;
            VerticalDistance = height * (3f / 4f);

            offset = (shouldOffset) ? width / 2 : 0;

            xposition = (column * (horizontalDistance)) + offset;
            yposition = (row * VerticalDistance);

        }
        else
        {
            shouldOffset = (column % 2) == 0;
            height = Mathf.Sqrt(3) * size;
            width = 2f * size;

            VerticalDistance = height;
            horizontalDistance = width * (3f / 4f);

            offset = (shouldOffset) ? height / 2 : 0;

            xposition = (column * horizontalDistance);
            yposition = (row * (VerticalDistance)) - offset;

        }

        return new Vector3(xposition,0,-yposition);
    }

    public void Redraw()
    {
        for (int x = 0; x < MapSize.x; x++)
        {
            for (int y = 0; y < MapSize.y; y++)
            {
                PlayArea.get_Tile(x, y).Hex.innerSize = innerSize;
                PlayArea.get_Tile(x, y).Hex.outerSize = outerSize;
                PlayArea.get_Tile(x, y).Hex.height = PlayArea.get_Tile(x, y).height;
                PlayArea.get_Tile(x, y).Hex.isFlatTopped = isFlatTopped;
                PlayArea.get_Tile(x, y).gameObject.transform.position = GetHexPositionFromCoordinate(new Vector2Int(x, y));
                PlayArea.get_Tile(x, y).SetMesh();
            }
        }
    }

    public void setFirstHex()
    {
        Vector3 center = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.scaledPixelHeight / 2, Camera.main.scaledPixelWidth / 2, 0));
        Tile Closest = null;
        float minDist = Mathf.Infinity;
        for (int x = 0; x < MapSize.x; x++)
        {
            for (int y = 0; y < MapSize.y; y++)
            {
                    float dist = Vector3.Distance(PlayArea.get_Tile(x,y).transform.position, center);
                    if (dist < minDist)
                    {
                        Closest = PlayArea.get_Tile(x, y);
                        minDist = dist;
                    }             
            }

        }
        EventManager.TileHoverTrigger(PlayArea.get_Tile(Closest.column, Closest.row).transform.gameObject);
    }
}
