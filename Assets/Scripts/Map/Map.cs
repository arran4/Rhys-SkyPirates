using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField]
    private Vector2Int MapSize;

    public float innerSize , outerSize, height;
    public bool isFlatTopped;

    [SerializeField]
    public List<TileDataSO> TileTypes;

    private Board PlayArea;

    //Using currently as a crude random map maker. Will probably have this build a map from a .json or two 
    void Start()
    {
        PlayArea = new Board(MapSize);
        int nuberofenemies = PawnManager.PawnManagerInstance.GetAllEnemies().Count -1;
        int qStart = -MapSize.x / 2;
        int rStart = -MapSize.y / 2;
        for (int x = 0; x < MapSize.x; x++)
        {
            for (int y = 0; y < MapSize.y; y++)
            {
                int q = qStart + x;
                int r = rStart + y;
                GameObject holder = new GameObject($"Hex {x},{y}", typeof(Tile));
                Tile tile = holder.GetComponent<Tile>();
                tile.Data = TileTypes[Random.Range(0, TileTypes.Count)];
                tile.SetPositionAndHeight(new Vector2Int(x, y), q, r, tile.Data == TileTypes[0] ? 5 : Random.Range(1, 7) * 5);
                Vector3 tilePosition = GetHexPositionFromCoordinate(new Vector2Int(x, y));
                tilePosition.y = tilePosition.y + tile.Height / 2;
                holder.transform.position = tilePosition;
                holder.transform.SetParent(transform);
                Instantiate(tile.Data.TilePrefab, holder.transform).transform.position += new Vector3(0, tile.Height / 2 -1, 0);
                tile.SetupHexRenderer(innerSize, outerSize, isFlatTopped);
                tile.SetPosition(new Vector2Int(x, y));
                PlayArea.set_Tile(x, y, tile);

                if (tile.Data.BaseMat == TileTypes[1].BaseMat && nuberofenemies >= 0)
                {
                    PawnManager.PawnManagerInstance.EnemyPawns[nuberofenemies].SetPosition(tile);
                    nuberofenemies--;
                }
            }
        }

        for (int x = 0; x < MapSize.x; x++)
        {
            for (int y = 0; y < MapSize.y; y++)
            {
                Tile NeighbourGet = PlayArea.get_Tile(x, y);
                NeighbourGet.Neighbours = PlayArea.GetNeighbours(new Vector2Int(NeighbourGet.Column, NeighbourGet.Row));
            }
        }
        setFirstHex();

    }

    //Sets a hexes possition in world coords from its x,y values
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

    //If for any reason the board needs to be entirely redrawn
    public void Redraw()
    {
        for (int x = 0; x < MapSize.x; x++)
        {
            for (int y = 0; y < MapSize.y; y++)
            {
                PlayArea.get_Tile(x, y).Hex.innerSize = innerSize;
                PlayArea.get_Tile(x, y).Hex.outerSize = outerSize;
                PlayArea.get_Tile(x, y).Hex.height = PlayArea.get_Tile(x, y).Height;
                PlayArea.get_Tile(x, y).Hex.isFlatTopped = isFlatTopped;
                PlayArea.get_Tile(x, y).gameObject.transform.position = GetHexPositionFromCoordinate(new Vector2Int(x, y));
                PlayArea.get_Tile(x, y).SetColliderMesh();
            }
        }
    }

    //sets first highlight assuming no mouse input
    public void setFirstHex()
    {
        // Calculate the center point of the screen
        Vector3 screenCenter = new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2, Camera.main.nearClipPlane);
        Vector3 center = Camera.main.ScreenToWorldPoint(screenCenter);

        Tile closest = null;
        float minDist = Mathf.Infinity;

        for (int x = 0; x < MapSize.x; x++)
        {
            for (int y = 0; y < MapSize.y; y++)
            {
                Tile currentTile = PlayArea.get_Tile(x, y);
                float dist = Vector3.Distance(currentTile.transform.position, center);
                if (dist < minDist)
                {
                    closest = currentTile;
                    minDist = dist;
                }
            }
        }

        if (closest != null)
        {
            EventManager.TileHoverTrigger(PlayArea.get_Tile(closest.Column, closest.Row).transform.gameObject);
        }
    }
    private Tile tile_add(Tile hex, int QAxis, int RAxis, int SAxis)
    {
        return PlayArea.SearchTileByCubeCoordinates(hex.QAxis + QAxis, RAxis + RAxis, hex.SAxis + SAxis);
    }
    public List<Tile> HexRing(Tile center, int radius)
    {
        List<Tile> results = new List<Tile>();

        for (int q = -radius; q <= radius; q++)
        {
            int r1 = Mathf.Max(-radius, -q - radius);
            int r2 = Mathf.Min(radius, -q + radius);
            for (int r = r1; r <= r2; r++)
            {
                int s = -q - r;
                results.Add(tile_add(center, q, r, s));
            }
        }

        return results;
    }
}
