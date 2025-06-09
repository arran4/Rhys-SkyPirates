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

    public Tile HexScale(Tile hex, int factor)
    {
        return _GameBoard.PlayArea.SearchTileByCubeCoordinates(hex.QAxis * factor, hex.RAxis * factor, hex.SAxis * factor);
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
                if (add != null)
                {
                    results.Add(add);
                }
            }
        }
        return results;
    }


    public List<Tile> HexRing(Tile center, int radius)
    {
        List<Tile> results = new List<Tile>();

        if (radius == 0)
        {
            return results;
        }

        Tile hex = HexScale(center.Neighbours[0], radius);

        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < radius; j++)
            {
                if (hex != null)
                {
                    results.Add(hex);
                    hex = hex.Neighbours[i];
                }
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
                if (neighbor != null && !visited.Contains(neighbor) && !(neighbor.Data.MovementCost == 0))
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

    public List<Tile> AreaLine(Tile center, Tile target, int range)
    {
        List<Tile> line = new List<Tile>();

        Vector3Int direction = _GameBoard.PlayArea.GetDirectionVector(center, target);
        if (direction == Vector3Int.zero) return line;

        Vector3Int current = center.ReturnSquareCoOrds();

        for (int i = 1; i <= range; i++)
        {
            current += direction;
            Tile tile = _GameBoard.PlayArea.SearchTileByCubeCoordinates(current.x, current.y, current.z);
            if (tile != null)
            {
                line.Add(tile);
            }
            else break; // Stop if we hit an invalid tile
        }

        return line;
    }



    public List<Tile> AreaCone(Tile center, int range, int direction)
    {
        List<Tile> cone = new List<Tile>();
        Tile current = center;

        if (direction < 0 || direction >= 6) return cone;

        for (int i = 1; i <= range; i++)
        {
            Tile main = current;
            for (int j = -1; j <= 1; j++)
            {
                int dir = (direction + j + 6) % 6;
                Tile step = main;
                for (int k = 0; k < i; k++)
                {
                    if (step == null) break;
                    step = step.Neighbours[dir];
                }
                if (step != null)
                    cone.Add(step);
            }

            current = current.Neighbours[direction];
            if (current == null) break;
        }

        return cone;
    }


}
