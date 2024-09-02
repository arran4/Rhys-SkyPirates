using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum RangeType
{
    Diagonal,
    Ring,
    Area,
    Line,
    Cone
}
public enum Affects
{
    Push,
    Pull,
    Drag,
    Bounce
}

public enum Effects
{
    Levitate,
    Poison,
    Paralize
}

public class Ability : ScriptableObject
{
    public List<RangeType> TotalArea;
    public List<int> Damage;
    public List<Affects> BoardChange;
    public List<Effects> PawnChange;


}
