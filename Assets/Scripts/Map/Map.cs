using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

[System.Serializable]
public class SerializableTile
{
    public string TileTypeID;
    public int Height;
}

[System.Serializable]
public class SerializableBoard
{
    public int x_Height;
    public int y_Width;
    public List<List<SerializableTile>> Tiles;
}

[System.Serializable]
public class ExportData
{
    public List<string> TileTypeIDs;
    public SerializableBoard Board;
}

public class Map : MonoBehaviour
{
    [SerializeField]
    private Vector2Int MapSize;

    public float innerSize , outerSize, height;
    public bool isFlatTopped;

    [SerializeField]
    public List<TileDataSO> TileTypes;

    public Board PlayArea;

    public Material HighlightMaterial;

    private MovementLine Arrow;

    //Using currently as a crude random map maker. Will probably have this build a map from a .json or two 
    public void Start()
    {
        Arrow = GetComponent<MovementLine>();
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
                tile.Data = TileTypes[Random.Range(0,2)];
                tile.SetPositionAndHeight(new Vector2Int(x, y), q, r, tile.Data == TileTypes[0] ? 5 : 20);
                Vector3 tilePosition = GetHexPositionFromCoordinate(new Vector2Int(x, y));
                tilePosition.y = tilePosition.y + tile.Height / 2;
                holder.transform.position = tilePosition;
                holder.transform.SetParent(transform);
                Instantiate(tile.Data.TilePrefab, holder.transform).transform.position += new Vector3(0, tile.Height / 2 -1, 0);
                tile.SetupHexRenderer(innerSize, outerSize, isFlatTopped);
                tile.SetPosition(new Vector2Int(x, y));
                tile.SetPawnPos();
                PlayArea.set_Tile(x, y, tile);

                if (tile.Data.BaseMat == TileTypes[1].BaseMat && nuberofenemies >= 0 && x > 4 && y > 2)
                {
                    PawnManager.PawnManagerInstance.EnemyPawns[nuberofenemies].SetPosition(tile);
                    nuberofenemies--;
                }
            }
        }

        int count = PlayerList.ListInstance.AllPlayerPawns.Count;
        while (count > 0)
        {
            Tile playerPos = PlayArea.get_Tile(Random.Range(0, MapSize.x), Random.Range(0, MapSize.y));
            if (playerPos.Data != TileTypes[0] && playerPos.Contents == null)
            {
                PawnManager.PawnManagerInstance.PlayerPawns[count - 1].SetPosition(playerPos);
                count--;
            }
        }

        Arrow.SetMap(PlayArea);
        SetNeighbours();
        setFirstHex();

    }

    //Sets a hexes possition in world coords from its x,y values
    public Vector3 GetHexPositionFromCoordinate(Vector2Int coordinates)
    {
        int q = coordinates.x;
        int r = coordinates.y;
        float size = outerSize;

        // Calculate the horizontal and vertical spacing between hexagons
        float width = Mathf.Sqrt(3) * size;
        float height = 2f * size;

        // Determine the horizontal offset based on the grid type
        float offset = isFlatTopped ? width / 2f : 0f;

        // Calculate the x and y positions based on axial coordinates
        float xPosition = (q * (width *0.90f) + offset);
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
            EventManager.TriggerTileHover(PlayArea.get_Tile(closest.Column, closest.Row).transform.gameObject);
        }
    }

    public void SetNeighbours()
    {
        for (int x = 0; x < MapSize.x; x++)
        {
            for (int y = 0; y < MapSize.y; y++)
            {
                Tile tile = PlayArea.get_Tile(x, y);
                
                foreach(Tile a in PlayArea.GetNeighbours(new Vector2Int(x, y)))
                {
                    tile.SetNeighbour(a);
                }
            }
        }
    }
    public void SaveMapToJson(string filePath)
    {
        var export = new ExportData();
        export.TileTypeIDs = new List<string>();
        Dictionary<TileDataSO, string> tileTypeLookup = new Dictionary<TileDataSO, string>();

        foreach (var tileType in TileTypes)
        {
            tileTypeLookup[tileType] = tileType.UniqueID;
            export.TileTypeIDs.Add(tileType.UniqueID);
        }

        export.Board = new SerializableBoard
        {
            x_Height = MapSize.x,
            y_Width = MapSize.y,
            Tiles = new List<List<SerializableTile>>()
        };

        for (int y = 0; y < MapSize.y; y++)
        {
            var row = new List<SerializableTile>();
            for (int x = 0; x < MapSize.x; x++)
            {
                Tile tile = PlayArea.get_Tile(x, y);
                row.Add(new SerializableTile
                {
                    TileTypeID = tileTypeLookup[tile.Data],
                    Height = (int)tile.Height
                });
            }
            export.Board.Tiles.Add(row);
        }

        string json = JsonConvert.SerializeObject(export, Formatting.Indented);
        File.WriteAllText(filePath, json);
        Debug.Log($"Map saved to: {filePath}");
    }

    public void LoadMapFromJson(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("File not found: " + filePath);
            return;
        }

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        string json = File.ReadAllText(filePath);
        ExportData data = JsonConvert.DeserializeObject<ExportData>(json);

        Dictionary<string, TileDataSO> idLookup = new Dictionary<string, TileDataSO>();
        foreach (var id in data.TileTypeIDs)
        {
            TileDataSO match = TileTypes.Find(t => t.UniqueID == id);
            if (match != null)
            {
                idLookup[id] = match;
            }
            else
            {
                Debug.LogWarning($"Missing TileDataSO with ID: {id}");
            }
        }
        foreach (var id in data.TileTypeIDs)
        {
            TileDataSO match = TileTypes.Find(t => t.UniqueID == id);
            if (match != null)
            {
                idLookup[id] = match;
                Debug.Log($"Mapped ID: {id} to TileDataSO: {match.name}");
            }
            else
            {
                Debug.LogWarning($"Missing TileDataSO with ID: {id}");
            }
        }

        PlayArea = new Board(new Vector2Int(data.Board.x_Height, data.Board.y_Width));
        for (int y = 0; y < data.Board.y_Width; y++)
        {
            for (int x = 0; x < data.Board.x_Height; x++)
            {
                SerializableTile sTile = data.Board.Tiles[y][x];
                TileDataSO tileType = idLookup.ContainsKey(sTile.TileTypeID) ? idLookup[sTile.TileTypeID] : TileTypes[0];

                GameObject holder = new GameObject($"Hex {x},{y}", typeof(Tile));
                Tile tile = holder.GetComponent<Tile>();
                tile.Data = tileType;
                tile.SetPositionAndHeight(new Vector2Int(x, y), x, y, sTile.Height);
                Vector3 tilePosition = GetHexPositionFromCoordinate(new Vector2Int(x, y));
                tilePosition.y += tile.Height / 2;
                holder.transform.position = tilePosition;
                holder.transform.SetParent(transform);
                Instantiate(tile.Data.TilePrefab, holder.transform).transform.position += new Vector3(0, tile.Height / 2 - 1, 0);
                tile.SetupHexRenderer(innerSize, outerSize, isFlatTopped);
                tile.SetPosition(new Vector2Int(x, y));
                tile.SetPawnPos();

                PlayArea.set_Tile(x, y, tile);
            }
        }

        SetNeighbours();
        setFirstHex();
        Debug.Log("Map loaded from JSON.");
        Debug.Log(filePath);
    }


}
