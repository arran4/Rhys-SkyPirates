using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRules : MonoBehaviour
{
    public static GameRules GameInstance { get; set; }
    public BasicControls inputActions { get; private set; }
    // Start is called before the first frame update

    public bool IsBeforeCapture;
    public bool IsPlus;
    public bool IsSame;
    public bool IsCombo;
    public bool IsRandom;
    public bool IsSwap;

    private void Awake()
    {
        if (GameInstance != null)
        {
#if UNITY_EDITOR
            DestroyImmediate(gameObject);
#else
    Destroy(gameObject);
#endif
            return;
        }

        inputActions = new BasicControls();
        //inputActions.Battle.Enable();
    }
    public void InitializeForTests()
    {
        Awake();
    }
}
