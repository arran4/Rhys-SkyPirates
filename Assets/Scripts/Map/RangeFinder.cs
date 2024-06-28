using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeFinder : MonoBehaviour
{
    public Map _GameBoard;
    // Start is called before the first frame update
    void Start()
    {
        _GameBoard = FindObjectOfType<Map>();
    }

    private Tile tile_add(Tile hex, int QAxis, int RAxis, int SAxis)
    {
        return _GameBoard.PlayArea.SearchTileByCubeCoordinates(hex.QAxis + QAxis, hex.RAxis + RAxis, hex.SAxis + SAxis);
    }

    public List<Tile> GetMovementRange(Pawn center)
    {
        List<Tile> MoveRange = AreaRing(center.Position, center.Stats.Movement);
        return MoveRange;
    }
    public List<Tile> AreaRing(Tile center, int radius)
    {
        List<Tile> results = new List<Tile>();

        for (int q = -radius; q <= radius; q++)
        {
            int r1 = Mathf.Max(-radius, -q - radius);
            int r2 = Mathf.Min(radius, -q + radius);
            for (int r = r1; r <= r2; r++)
            {
                int s = -q - r;
                Tile add = tile_add(center, q, r, s);
                if (add != null && add.Data.MovementCost != 0)
                {
                    results.Add(add);
                }
            }
        }
        return results;
    }

    public Tile HexScale(Tile hex, int factor)
    {
        return _GameBoard.PlayArea.SearchTileByCubeCoordinates(hex.QAxis * factor, hex.RAxis * factor, hex.SAxis * factor);
    }

    public List<Tile> HexRing(Tile center, int radius)
    {
        List<Tile> results = new List<Tile>();

        if (radius == 0)
        {
            return results;
        }

        Tile hex2 = HexScale(center.Neighbours[4], radius);
        Tile hex = tile_add(center, hex2.QAxis,hex2.RAxis, hex2.SAxis);

        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < radius; j++)
            {
                results.Add(hex);
                hex = hex.Neighbours[i];
            }
        }

        return results;
    }

    public List<Tile> HexReachable(Tile start, int movement)
    {
        HashSet<Tile> visited = new HashSet<Tile>();
        Queue<(Tile tile, int cost)> fringes = new Queue<(Tile, int)>();

        visited.Add(start);
        fringes.Enqueue((start, 0));

        while (fringes.Count > 0)
        {
            var (currentTile, currentCost) = fringes.Dequeue();

            foreach (Tile neighbor in currentTile.Neighbours)
            {
                if (neighbor != null && !visited.Contains(neighbor) && !IsBlocked(neighbor))
                {
                    int newCost = currentCost + neighbor.Data.MovementCost;
                    if (newCost <= movement && neighbor.Contents == null)
                    {
                        visited.Add(neighbor);
                        fringes.Enqueue((neighbor, newCost));
                    }
                }
            }
        }

        return new List<Tile>(visited);
    }

    private bool IsBlocked(Tile tile)
    {
        // A tile is considered blocked if its movement cost is zero
        return tile.Data.MovementCost == 0;
    }

}
