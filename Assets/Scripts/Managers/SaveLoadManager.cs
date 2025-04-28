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

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager SaveLoadInstance { get; private set; }
    public void SaveMapToJson(Map map, string filePath)
    {
        var export = new ExportData();
        export.TileTypeIDs = new List<string>();
        Dictionary<TileDataSO, string> tileTypeLookup = new Dictionary<TileDataSO, string>();

        foreach (var tileType in map.TileTypes)
        {
            tileTypeLookup[tileType] = tileType.UniqueID;
            export.TileTypeIDs.Add(tileType.UniqueID);
        }

        export.Board = new SerializableBoard
        {
            x_Height = map.MapSize.x,
            y_Width = map.MapSize.y,
            Tiles = new List<List<SerializableTile>>()
        };

        for (int y = 0; y < map.MapSize.y; y++)
        {
            var row = new List<SerializableTile>();
            for (int x = 0; x < map.MapSize.x; x++)
            {
                Tile tile = map.PlayArea.get_Tile(x, y);
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

    public void LoadMapFromJson(Map map, string filePath)
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
            TileDataSO match = map.TileTypes.Find(t => t.UniqueID == id);
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
            TileDataSO match = map.TileTypes.Find(t => t.UniqueID == id);
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

        map.PlayArea = new Board(new Vector2Int(data.Board.x_Height, data.Board.y_Width));
        for (int y = 0; y < data.Board.y_Width; y++)
        {
            for (int x = 0; x < data.Board.x_Height; x++)
            {
                SerializableTile sTile = data.Board.Tiles[y][x];
                TileDataSO tileType = idLookup.ContainsKey(sTile.TileTypeID) ? idLookup[sTile.TileTypeID] : map.TileTypes[0];

                GameObject holder = new GameObject($"Hex {x},{y}", typeof(Tile));
                Tile tile = holder.GetComponent<Tile>();
                tile.Data = tileType;
                tile.SetPositionAndHeight(new Vector2Int(x, y), x, y, sTile.Height);
                Vector3 tilePosition = map.GetHexPositionFromCoordinate(new Vector2Int(x, y));
                tilePosition.y += tile.Height / 2;
                holder.transform.position = tilePosition;
                holder.transform.SetParent(transform);
                Instantiate(tile.Data.TilePrefab, holder.transform).transform.position += new Vector3(0, tile.Height / 2 - 1, 0);
                tile.SetupHexRenderer(map.innerSize, map.outerSize, map.isFlatTopped);
                tile.SetPosition(new Vector2Int(x, y));
                tile.SetPawnPos();

                map.PlayArea.set_Tile(x, y, tile);
            }
        }

        map.SetNeighbours();
        map.setFirstHex();
        Debug.Log("Map loaded from JSON.");
        Debug.Log(filePath);
    }

    private void Awake()
    {
        if (SaveLoadInstance != null && SaveLoadInstance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        SaveLoadInstance = this;

    }
}
