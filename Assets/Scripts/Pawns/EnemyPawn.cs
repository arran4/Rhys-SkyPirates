using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPawn : Pawn
{
    public override void SetPosition(Tile Set)
    {
        Position = Set;
        transform.position = new Vector3( Set.transform.position.x, Set.transform.position.y + (Set.Height * 5/2), Set.transform.position.z);        
    }
}
