using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaterSelect : MonoBehaviour
{

    public Transform OnScreen;
    public Transform Storage;
    public List<PlayerPawns> PlayerPawnsList;
    public int PawnOnScreen = 0;
    // Start is called before the first frame update
    void Start()
    {
        PlayerPawnsList = new List<PlayerPawns>();
        foreach (PlayerPawns x in PawnManager.PawnManagerInstance.PlayerPawns)
        {
            x.gameObject.transform.position = Storage.position;
            PlayerPawnsList.Add(x);
        }

        PlayerPawnsList[0].gameObject.transform.position = OnScreen.position;
        PawnOnScreen = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
