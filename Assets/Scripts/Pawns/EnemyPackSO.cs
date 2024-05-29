using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EType 
{ 
    Corrsair,
    Cannoneir,
    Rifleman
}

[CreateAssetMenu(fileName = "Enemy", menuName = "ScriptableObject/EnemyPack")]
public class EnemyPackSO : ScriptableObject
{

    public List<int> NumberInPack;

    public List<EType> EnemyType;

}
