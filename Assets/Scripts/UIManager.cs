using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public int MonsterKilled;

    void Start()
    {
        MonsterKilled = 0;
    }

    public void kill()
    {
        MonsterKilled++;
    }
}
