using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public int MonsterKilled;
    public int TotalMonsters;

    void Start()
    {
        MonsterKilled = 0;
        TotalMonsters = 200;
    }

    public void kill()
    {
        MonsterKilled++;
    }
}
