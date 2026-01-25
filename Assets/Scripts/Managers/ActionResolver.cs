using System.Collections.Generic;
using UnityEngine;

public static class ActionResolver
{
    public static void ApplyForcedMovement(Pawn target, Tile originTile, Effect type, int distance, Board board)
    {
        if (target == null || board == null || distance <= 0) return;

        Tile currentTile = target.Position;
        if (currentTile == null) return;

        Vector3Int pushDirection = Vector3Int.zero;

        // Determine Direction
        if (type == Effect.Push)
        {
            pushDirection = board.GetDirectionVector(originTile, currentTile);
            if (pushDirection == Vector3Int.zero) return;
        }
        else if (type == Effect.Pull)
        {
            pushDirection = board.GetDirectionVector(currentTile, originTile);
            if (pushDirection == Vector3Int.zero) return;
        }
        else if (type == Effect.Slide)
        {
            Vector3Int dir = board.GetDirectionVector(originTile, currentTile);
            if (dir == Vector3Int.zero) return;

            int index = System.Array.IndexOf(HexUtils.CubeDirections, dir);
            if (index == -1) return;

            // Slide Left (CCW) relative to Push direction.
            // This moves the pawn perpendicular to the line between Origin and Target.
            // Consistent with "forces the affected pawn to the source's left or right".
            pushDirection = HexUtils.CubeDirections[(index + 1) % 6];
        }
        else
        {
            return;
        }

        // Apply Movement
        for (int i = 0; i < distance; i++)
        {
            Tile nextTile = board.GetNeighbourInDirection(currentTile, pushDirection);

            // 1. Check Sky / Bounds
            bool isSky = (nextTile == null);
            if (nextTile != null && !nextTile.Data.Walkable)
            {
                isSky = true;
            }

            if (isSky)
            {
                // Move into the sky tile if it exists
                if (nextTile != null)
                {
                    currentTile.Contents = null;
                    target.SetPosition(nextTile);
                    currentTile = nextTile;
                }

                // If nextTile is null, we can't move off-grid.
                // We stay at currentTile (Edge) but enter Rescue Mode.

                HandleRescue(target, currentTile, board);
                return; // Stop movement
            }

            // 2. Check Elevation
            float heightDiff = nextTile.Height - currentTile.Height;
            if (heightDiff > 1.5f)
            {
                 ApplyDamage(target, 5, DamageType.Earth);
                 return;
            }

            // 3. Check Pawn Collision
            if (nextTile.Contents != null)
            {
                ApplyDamage(target, 3, DamageType.Earth);
                ApplyDamage(nextTile.Contents, 3, DamageType.Earth);
                return;
            }

            // Move
            currentTile.Contents = null;
            target.SetPosition(nextTile);
            currentTile = nextTile;
        }
    }

    private static void HandleRescue(Pawn target, Tile currentTile, Board board)
    {
         target.EnterRescueMode();

         // Check if current tile (Sky/Edge) has walkable neighbors (Rescue possible?)
         bool hasWalkableNeighbor = false;
         foreach(Tile n in currentTile.Neighbours)
         {
             if (n != null && n.Data.Walkable) hasWalkableNeighbor = true;
         }

         if (!hasWalkableNeighbor)
         {
             // Deep sky / Isolated. Find nearest valid hex.
             Tile nearest = FindNearestWalkable(currentTile, board);
             if (nearest != null)
             {
                 // We want to be "next to" nearest.
                 // Find a neighbor of `nearest` that is !Walkable (Sky).
                 Tile bestSpot = null;
                 foreach(Tile n in nearest.Neighbours)
                 {
                     if (n != null && !n.Data.Walkable && n.Contents == null)
                     {
                         bestSpot = n;
                         break;
                     }
                 }

                 if (bestSpot != null)
                 {
                     currentTile.Contents = null;
                     target.SetPosition(bestSpot);
                 }
                 else
                 {
                    // Fallback: Place on nearest valid.
                    currentTile.Contents = null;
                    target.SetPosition(nearest);
                 }
             }
         }
    }

    private static Tile FindNearestWalkable(Tile start, Board board)
    {
        Queue<Tile> q = new Queue<Tile>();
        HashSet<Tile> visited = new HashSet<Tile>();
        q.Enqueue(start);
        visited.Add(start);

        int count = 0;

        while(q.Count > 0 && count < 1000)
        {
            count++;
            Tile current = q.Dequeue();
            if (current != start && current.Data.Walkable && current.Contents == null) return current;

            foreach(Tile n in current.Neighbours)
            {
                if (n != null && !visited.Contains(n))
                {
                    visited.Add(n);
                    q.Enqueue(n);
                }
            }
        }
        return null;
    }

    private static void ApplyDamage(Pawn p, int amount, DamageType type)
    {
        if (p == null) return;
        IHealth h = p.GetComponent<IHealth>();
        if (h != null) h.TakeDamage(amount, type);
    }
}
