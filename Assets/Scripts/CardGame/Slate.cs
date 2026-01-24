using UnityEngine;
public class Slate 
{
    private SOCard Data;

    public string Name;

    public int top, left, right, bottom;
    public Sprite CardFace;

    public Slate (SOCard Data)
    {
        this.Data = Data;
        top = Data.Top;
        left = Data.Left;
        right = Data.Right;
        bottom = Data.Bottom;
        CardFace = Data.CardFace;
        Name = Data.Name;
    }


    public override string ToString()
    {
        return Data.name;
    }

}
