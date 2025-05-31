using UnityEngine;

public class GenerateMerge : MonoBehaviour, IGenerate
{
    public string Ship1Local;
    public string Ship2Local;
    public ShipSide Side;
    public BoardRotator.Rotation Rot;

    public Board Generate(Map Data)
    {
        Board Ship1 = SaveLoadManager.LoadBoardFromJson(Application.persistentDataPath + "/" + Ship1Local + ".json", Data, Data.transform);
        Board Ship2 = SaveLoadManager.LoadBoardFromJson(Application.persistentDataPath + "/" + Ship2Local + ".json", Data, Data.transform);

        Ship2 = BoardRotator.RotateBoard(Ship2, Rot);

        MapMerge.MergeBoards(Data, Ship1, Ship2, Side);

        Ship1.Destroy();
        Ship2.Destroy();

        return Data.PlayArea;
    }
}
