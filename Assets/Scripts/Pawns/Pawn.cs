using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pawn : MonoBehaviour
{
    public GameObject BaseObject;
    public Tile Position;

    public abstract void SetPosition(Tile Set);
}
