using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board 
{
    public int _size_X { get; private set; }
    public int _size_Y { get; private set; }

    private Tile[,] _board_Contents;

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
}
