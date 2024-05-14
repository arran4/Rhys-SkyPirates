using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board 
{
    public int _size_X { get; private set; }
    public int _size_Y { get; private set; }

    private Tile[,] _board_Contents;

    private Vector2Int[] EvenNeighbours = new Vector2Int[6] { new Vector2Int(1, 0), new Vector2Int(1, -1), new Vector2Int(0, -1), new Vector2Int(-1, -1), new Vector2Int(-1, 0), new Vector2Int(0, 1) };
    private Vector2Int[] OddNeighbours = new Vector2Int[6] { new Vector2Int(1, 1), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(-1, 0), new Vector2Int(-1, 1), new Vector2Int(0, 1) };

    public Board(Vector2Int coordinates)
    {
        _size_Y = coordinates.y;
        _size_X = coordinates.x;
        _board_Contents = new Tile[_size_X,_size_Y]; 
    }

    public Tile get_Tile(int x, int y)
    {
        return _board_Contents[x, y];
    }

    public void set_Tile(int x, int y, Tile toset)
    {
        _board_Contents[x, y] = toset;
    }

    public void swap_Tiles(Vector2Int Tile1, Vector2Int Tile2)
    {
        Tile placeholder = get_Tile(Tile1.x, Tile1.y);
        set_Tile(Tile1.x, Tile1.y, get_Tile(Tile2.x, Tile2.y));
        set_Tile(Tile2.x, Tile2.y, placeholder);
    }

    public List<Tile> GetNeighbours(Vector2Int centerTile)
    {
        List<Tile> Neighbours = new List<Tile>();
        if(centerTile.x % 2 == 0)
        {
            foreach(Vector2Int neighbour in EvenNeighbours)
            {
                if ((centerTile.x + neighbour.x >= 0 && centerTile.x + neighbour.x < _size_X) && (centerTile.y + neighbour.y >= 0 && centerTile.y + neighbour.y < _size_Y))
                {
                    Neighbours.Add(get_Tile(centerTile.x + neighbour.x, centerTile.y + neighbour.y));
                }
            }
        }
        else
        {
            foreach (Vector2Int neighbour in OddNeighbours)
            {
                if ((centerTile.x + neighbour.x > 0 && centerTile.x + neighbour.x < _size_X) && (centerTile.y + neighbour.y > 0 && centerTile.y + neighbour.y < _size_Y))
                {
                    Neighbours.Add(get_Tile(centerTile.x + neighbour.x, centerTile.y + neighbour.y));
                }
            }
        }
        return Neighbours;
    }
}
