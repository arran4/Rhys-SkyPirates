using UnityEngine;

public enum CardEffectType
{
    None,

    RotateClockwise,
    RotateCounterclockwise,

    FlipXAxis,
    FlipYAxis,

    MoveSlateToPosition,
    SwapWithSlate
}

public enum SlateTarget
{
    TopLeft,
    TopCenter,
    TopRight,

    MiddleLeft,
    Center,
    MiddleRight,

    BottomLeft,
    BottomCenter,
    BottomRight
}



[CreateAssetMenu(fileName = "Card", menuName = "ScriptableObject/Card")]
public class SOCard : ScriptableObject
{
    public string Name;

    public int Top;
    public int Left;
    public int Right;
    public int Bottom;

    public Sprite CardFace;

    public CardEffectType Effect;

    [Header("Move Slate Settings")]
    public SlateTarget SlateTarget;
}
