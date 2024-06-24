using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board
{
    public int _size_X { get; private set; }
    public int _size_Y { get; private set; }

    private Tile[,] _board_Contents;

    //Additive vectors for assigning neighbours 
    private Vector3Int[] directions = new Vector3Int[6] { new Vector3Int(1, -1, 0), new Vector3Int(1, 0, -1), new Vector3Int(0, 1, -1),new Vector3Int(-1, 1, 0), new Vector3Int(-1, 0, 1), new Vector3Int(0, -1, 1) };
   
    public Board(Vector2Int coordinates)
    {
        _size_Y = coordinates.y;
        _size_X = coordinates.x;
        _board_Contents = new Tile[_size_X, _size_Y];
    }

    public Tile get_Tile(int x, int y)
    {
        return _board_Contents[x, y];
    }

    public void set_Tile(int x, int y, Tile toset)
    {
        toset.SetPosition(new Vector2Int(x, y));
        _board_Contents[x, y] = toset;
    }

    public void swap_Tiles(Vector2Int Tile1, Vector2Int Tile2)
    {
        Tile placeholder = get_Tile(Tile1.x, Tile1.y);
        set_Tile(Tile1.x, Tile1.y, get_Tile(Tile2.x, Tile2.y));
        set_Tile(Tile2.x, Tile2.y, placeholder);
    }

    //Sets the 6 neighbours of each tile on the board
    public List<Tile> GetNeighbours(Vector2Int centerTile)
    {
        List<Tile> Neighbours = new List<Tile>();

        Tile tile = get_Tile(centerTile.x, centerTile.y);

        for (int i = 0; i < 6; i++)
        {
            Vector3Int neighborPos = new Vector3Int(tile.QAxis + directions[i].x, tile.RAxis + directions[i].y, tile.SAxis + directions[i].z);
            Tile neighbour = SearchTileByCubeCoordinates(neighborPos.x, neighborPos.y, neighborPos.z);
            if (neighbour != null)
            {
                Neighbours.Add(neighbour);
            }
        }

        return Neighbours;
    }
    public Tile SearchTileByCubeCoordinates(int q, int r, int s)
    {
        int centerX = _size_X / 2;
        int centerY = _size_Y / 2;

        // Convert cube coordinates (q, r, s) to array indices (x, y)
        // This conversion depends on how your array and cube coordinates are related
        int x = q + centerX;
        int y = r + centerY;

        // Check if the calculated indices are within bounds
        if (x >= 0 && x < _board_Contents.GetLength(0) && y >= 0 && y < _board_Contents.GetLength(1))
        {
            return _board_Contents[x, y];
        }
        else
        {
            // If the indices are out of bounds, return null
            return null;
        }
    }
}
