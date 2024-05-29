using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnManager : MonoBehaviour
{
    public static PawnManager PawnManagerInstance { get; private set; }
    private List<GameObject> PlayerPawns;
    public List<GameObject> EnemyPawns;

    public List<GameObject> GetAllEnemies()
    {
        return EnemyPawns;
    }

    public int getPlayerPawnPosition(GameObject Pawn)
    {
        int count = 0;
        foreach(GameObject n in PlayerPawns)
        {
            if(n == Pawn)
            {
                return count;
            }
            count++;
        }

        return -1;
    }

    public int getEnemyPawnPosition(GameObject Pawn)
    {
        int count = 0;
        foreach (GameObject n in EnemyPawns)
        {
            if (n == Pawn)
            {
                return count;
            }
            count++;
        }

        return -1;
    }

    public GameObject getPlayerPawn(int Index)
    {
        return PlayerPawns[Index];
    }

    public GameObject getEnemyPawn(int Index)
    {
        return EnemyPawns[Index];
    }

    public GameObject getPawn(GameObject Pawn)
    {
        foreach(GameObject n in PlayerPawns)
        {
            if(n == Pawn)
            {
                return n;
            }
        }
        foreach(GameObject n in EnemyPawns)
        {
            if(n == Pawn)
            {
                return n;
            }    
        }

        return null;
    }    

    public int GetNextPlayerPawnByObject(GameObject Index)
    {
        int count = 0;
        foreach(GameObject n in PlayerPawns)
        {
            if (n == Index)
            {
                if(count == PlayerPawns.Count)
                {
                    return 0;
                }
                else
                {
                    return count;
                }
            }
        }
        return -1;
    }

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (PawnManagerInstance != null && PawnManagerInstance != this)
        {
            Destroy(this);
        }
        else
        {
            PawnManagerInstance = this;
        }
        PlayerPawns = new List<GameObject>();
        EnemyPawns = new List<GameObject>();
    }

    public void populateEnemey(List<GameObject> Enemy)
    {
        foreach(GameObject n in Enemy)
        {
            EnemyPawns.Add(n);
        }
    }

    public void populatePlayer(List<GameObject> Player)
    {
        foreach(GameObject n in Player)
        {
            PlayerPawns.Add(n);
        }
    }

    public void clearPawns()
    {
        PlayerPawns.Clear();
        EnemyPawns.Clear();
    }
}
