using UnityEngine;

[CreateAssetMenu(fileName = "TileData", menuName = "ScriptableObject/TileData")]
public class TileDateSO : ScriptableObject
{
    [SerializeField]
    public int MovementCost;

    [SerializeField]
    public GameObject TilePrefab;

    [SerializeField]
    public bool Walkable;

    [SerializeField]
    public Material BaseMat;

}
