using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum RangeType
{
    Single,
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

public enum ActionTypes
{
    Damage,
    Heal,
    Nutral
}

public enum Effects
{
    Levitate,
    Poison,
    Paralize
}

[CreateAssetMenu(fileName = "Ability", menuName = "ScriptableObject/Ability")]
public class Ability : ScriptableObject
{
    public string Name;
    public int Range;
    public List<RangeType> TotalArea;
    public List<int> Size;
    public List<ActionTypes> Action;
    public List<int> HealthChange;
    public List<Affects> BoardChange;
    public List<Effects> PawnChange;


}
