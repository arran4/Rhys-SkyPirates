using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerList : MonoBehaviour
{
    public static PlayerList ListInstance { get; private set; }

    public List<GameObject> AllPlayerPawns;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (ListInstance != null && ListInstance != this)
        {
            Destroy(this);
        }
        else
        {
            ListInstance = this;
        }
        DontDestroyOnLoad(this);
    }
}
