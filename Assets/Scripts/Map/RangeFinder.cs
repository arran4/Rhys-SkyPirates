using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// MonoBehaviour wrapper that delegates range calculations to
/// <see cref="RangeCalculator"/>. The heavy math was extracted into that static
/// class so the logic can be tested without requiring a running scene.
/// Keeping this behaviour separate also lets AI coding agents analyse and
/// extend the algorithms more easily.
/// </summary>
public class RangeFinder : MonoBehaviour
{
    /// <summary>
    /// Reference to the <see cref="Map"/> component containing the active board.
    /// If not assigned in the inspector it will be located at runtime.
    /// </summary>
    [SerializeField]
    public Map _GameBoard;

    void Awake()
    {
        if (_GameBoard == null)
        {
            _GameBoard = FindObjectOfType<Map>();
        }
    }

    /// <summary>
    /// Wrapper for <see cref="RangeCalculator.HexScale"/> using the active board.
    /// </summary>
    public Tile HexScale(Tile tile, int factor)
    {
        return RangeCalculator.HexScale(_GameBoard.PlayArea, tile, factor);
    }

    /// <summary>
    /// Wrapper for <see cref="RangeCalculator.AreaRing"/>.
    /// </summary>
    public List<Tile> AreaRing(Tile center, int radius)
    {
        return RangeCalculator.AreaRing(_GameBoard.PlayArea, center, radius);
    }

    /// <summary>
    /// Wrapper for <see cref="RangeCalculator.HexRing"/>.
    /// </summary>
    public List<Tile> HexRing(Tile center, int radius)
    {
        return RangeCalculator.HexRing(_GameBoard.PlayArea, center, radius);
    }

    /// <summary>
    /// Wrapper for <see cref="RangeCalculator.HexReachable"/>.
    /// </summary>
    public List<Tile> HexReachable(Tile start, int movement)
    {
        return RangeCalculator.HexReachable(_GameBoard.PlayArea, start, movement);
    }

    /// <summary>
    /// Wrapper for <see cref="RangeCalculator.AreaLine"/>.
    /// </summary>
    public List<Tile> AreaLine(Tile center, Tile target, int range)
    {
        return RangeCalculator.AreaLine(_GameBoard.PlayArea, center, target, range);
    }

    /// <summary>
    /// Wrapper for <see cref="RangeCalculator.AreaCone"/>.
    /// </summary>
    public List<Tile> AreaCone(Tile center, int range, int direction)
    {
        return RangeCalculator.AreaCone(_GameBoard.PlayArea, center, range, direction);
    }
}
