using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private GameObject spawnedEnemy;
    void Start()
    {
        StartCoroutine(SpawnMonsters());
    }
    IEnumerator SpawnMonsters()
    {

        while (true)
        {
            yield return new WaitForSeconds(Random.Range(1, 5));

            spawnedEnemy = PoolingManager.Instantiate(PoolingManager.Type.Enemy, PrefabsManager.GetPrefab(PrefabsManager.PrefabType.Enemy), transform.position, Quaternion.identity);
            spawnedEnemy.GetComponent<Enemy>().speed = 5;
            AudioManager.Play(AudioManager.ClipType.MonsterNoise);
        }
    }
}
