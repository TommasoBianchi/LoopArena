using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Rect SpawnArea;
    private GameObject spawnedEnemy;
    private UIManager UI;
    void Start()
    {
        UI = FindObjectOfType<UIManager>();
        StartCoroutine(SpawnMonsters());
    }
    IEnumerator SpawnMonsters()
    {

        for (int i = 0; i <= UI.TotalMonsters; i++)
        {
            yield return new WaitForSeconds(Random.Range(1, 5));

            if (SpawnArea.Contains(transform.position))
            {
                spawnedEnemy = PoolingManager.Instantiate(PoolingManager.Type.Enemy, PrefabsManager.GetPrefab(PrefabsManager.PrefabType.Enemy), transform.position, Quaternion.identity);
                spawnedEnemy.GetComponent<Enemy>().speed = 5;
                spawnedEnemy.GetComponent<Enemy>().health = 2;
                AudioManager.Play(AudioManager.ClipType.MonsterNoise);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(new Vector2(SpawnArea.xMin, SpawnArea.yMin), new Vector2(SpawnArea.xMin, SpawnArea.yMax));
        Gizmos.DrawLine(new Vector2(SpawnArea.xMin, SpawnArea.yMax), new Vector2(SpawnArea.xMax, SpawnArea.yMax));
        Gizmos.DrawLine(new Vector2(SpawnArea.xMax, SpawnArea.yMax), new Vector2(SpawnArea.xMax, SpawnArea.yMin));
        Gizmos.DrawLine(new Vector2(SpawnArea.xMax, SpawnArea.yMin), new Vector2(SpawnArea.xMin, SpawnArea.yMin));
    }
}
