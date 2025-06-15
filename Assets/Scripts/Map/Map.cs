using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class Map : MonoBehaviour
{
    [SerializeField]
    public Vector2Int MapSize;

    public float innerSize , outerSize, height;
    public bool isFlatTopped;

    [SerializeField]
    public List<TileDataSO> TileTypes;

    public Board PlayArea;

    public IGenerate generate;

    public Material HighlightMaterial;

    private MovementLine Arrow;

    //Generation types implemented working on merging two ships 
    public void Start()
    {
        generate = GetComponent<IGenerate>();
        Arrow = GetComponent<MovementLine>();
        PlayArea = generate.Generate(this);
       // int nuberofenemies = PawnManager.PawnManagerInstance.GetAllEnemies().Count - 1;

        Arrow.SetMap(PlayArea);
        SetNeighbours(PlayArea, isFlatTopped);
        setFirstHex();

        if (!typeof(GenerateMerge).IsInstanceOfType(generate))
        {
            foreach(PlayerPawns x in PawnManager.PawnManagerInstance.PlayerPawns)
            {
                x.gameObject.SetActive(false);
            }
            return;
        }
        int count = PlayerList.ListInstance.AllPlayerPawns.Count;
        while (count > 0)
        {
            Tile playerPos = PlayArea.get_Tile(Random.Range(0, MapSize.x), Random.Range(0, MapSize.y));
            if (playerPos != null)
            {
                if (playerPos.Data != TileTypes[0] && playerPos.Contents == null)
                {
                    PawnManager.PawnManagerInstance.PlayerPawns[count - 1].SetPosition(playerPos);
                    count--;
                }
            }
        }
        count = PawnManager.PawnManagerInstance.EnemyPawns.Count;
        while (count > 0)
        {
            Tile EnemyPos = PlayArea.get_Tile(Random.Range(0, MapSize.x), Random.Range(0, MapSize.y));
            if (EnemyPos != null)
            {
                if (EnemyPos.Data != TileTypes[0] && EnemyPos.Contents == null)
                {
                    PawnManager.PawnManagerInstance.EnemyPawns[count - 1].SetPosition(EnemyPos);
                    count--;
                }
            }
        }
    }



    /// <summary>
    /// Converts axial grid coordinates <c>(q,r)</c> into Unity world space.
    /// <para>
    /// The width and height of a hexagon are derived from the outer radius
    /// <c>size</c> as:
    /// <code>
    /// width  = sqrt(3) * size
    /// height = 2 * size
    /// </code>
    /// For flat topped layouts the origin is shifted horizontally by
    /// <c>width / 2</c>. A <c>0.9</c> factor leaves a small gap between tiles.
    /// </para>
    /// <para>Sample positions when <c>size = 1</c>:</para>
    /// <code>
    /// | q | r | layout | expected (x,z) |
    /// | 0 | 0 | flat   | (0.866,  0.000) |
    /// | 1 | 0 | flat   | (2.425, -0.900) |
    /// | 0 | 1 | flat   | (0.866, -1.800) |
    /// | 0 | 0 | pointy | (0.000,  0.000) |
    /// | 1 | 0 | pointy | (1.559, -0.900) |
    /// | 1 | 1 | pointy | (1.559, -2.700) |
    /// </code>
    /// <para>Coordinate diagram:</para>
    /// <code>
    ///    (-1,1)  (0,1)  (1,1)
    ///       \      |      /
    ///    (-1,0) (0,0) (1,0)
    ///       /      |      \
    ///    (-1,-1)(0,-1)(1,-1)
    /// </code>
    /// </summary>
    public Vector3 GetHexPositionFromCoordinate(Vector2Int coordinates)
    {
        int q = coordinates.x;
        int r = coordinates.y;
        float size = outerSize;

        float width = Mathf.Sqrt(3) * size;
        float height = 2f * size;

        float offset = isFlatTopped ? width / 2f : 0f;

        float xPosition = (q * (width * 0.90f) + offset);
        float yPosition = -((r + q / 2f) * height * 0.90f);

        return new Vector3(xPosition, 0f, yPosition);
    }

    //If for any reason the board needs to be entirely redrawn
    public void Redraw()
    {
        for (int x = 0; x < MapSize.x; x++)
        {
            for (int y = 0; y < MapSize.y; y++)
            {
                Tile Draw = PlayArea.get_Tile(x, y);
                if (Draw != null)
                {
                    Draw.Hex.innerSize = innerSize;
                    Draw.Hex.outerSize = outerSize;
                    Draw.Hex.height = PlayArea.get_Tile(x, y).Height;
                    Draw.Hex.isFlatTopped = isFlatTopped;
                    Draw.gameObject.transform.position = GetHexPositionFromCoordinate(new Vector2Int(x, y));
                    Draw.SetColliderMesh();
                }
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
                if (currentTile != null)
                {
                    float dist = Vector3.Distance(currentTile.transform.position, center);
                    if (dist < minDist)
                    {
                        closest = currentTile;
                        minDist = dist;
                    }
                }
            }
        }

        if (closest != null)
        {
            EventManager.TriggerTileHover(PlayArea.get_Tile(closest.Column, closest.Row).transform.gameObject);
        }
    }

    public static readonly Vector2Int[] NeighborOffsetsFlatTopped = new Vector2Int[]
    {
    new Vector2Int(+1, 0), new Vector2Int(0, +1), new Vector2Int(-1, +1),
    new Vector2Int(-1, 0), new Vector2Int(0, -1), new Vector2Int(+1, -1)
    };

    public static readonly Vector2Int[] NeighborOffsetsPointyTopped = new Vector2Int[]
    {
    new Vector2Int(0, +1), new Vector2Int(-1, 0), new Vector2Int(-1, -1),
    new Vector2Int(0, -1), new Vector2Int(+1, 0), new Vector2Int(+1, +1)
    };

    public void SetNeighbours(Board board, bool isFlatTopped)
    {
        var neighborOffsets = isFlatTopped ? NeighborOffsetsFlatTopped : NeighborOffsetsPointyTopped;

        foreach (Tile tile in board.GetAllTiles())
        {
            if (tile == null) continue;

            Vector2Int pos = new Vector2Int(tile.Column, tile.Row);

            foreach (var offset in neighborOffsets)
            {
                Vector2Int neighborPos = pos + offset;
                Tile neighborTile = board.get_Tile(neighborPos.x, neighborPos.y);
                if (neighborTile != null)
                {
                    tile.SetNeighbour(neighborTile);
                }
            }
        }
    }



    public void setSingleNeighbour(int x, int y)
    {
        Tile tile = PlayArea.get_Tile(x, y);

        foreach (Tile a in PlayArea.GetNeighbours(new Vector2Int(x, y)))
        {
            tile.SetNeighbour(a);
        }
    }
}
