using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class HexSelectState
{
    public abstract void EnterState(HexSelectManager manager);
    public abstract void UpdateState(HexSelectManager manager);
    public abstract void ExitState(HexSelectManager manager);
}