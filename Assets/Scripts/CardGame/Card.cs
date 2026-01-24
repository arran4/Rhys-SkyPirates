public class Card : Slate
{
    public CardEffectType Effect;
    public SlateTarget SlateTarget;

    public bool Capture;

    public Card(SOCard Data, bool capture) : base(Data)
    {
        Capture = capture;
        Effect = Data.Effect;
        SlateTarget = Data.SlateTarget;
    }
}
