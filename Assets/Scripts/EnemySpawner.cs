using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Rect SpawnArea;
    private GameObject spawnedEnemy;
    private Player player;
    private UIManager ui;
    void Start()
    {
        player = FindObjectOfType<Player>();
        ui = FindObjectOfType<UIManager>();
        StartCoroutine(SpawnMonsters());
    }
    IEnumerator SpawnMonsters()
    {
        while (ui.MonsterKilled <= ui.TotalMonsters)
        {
            yield return new WaitForSeconds(1);

            Vector3 randomSpawnPosition = new Vector3(player.transform.position.x + (Random.Range(0, 2) * 40 - 20), player.transform.position.y + (Random.Range(0, 2) * 40 - 20), 0);

            if (SpawnArea.Contains(randomSpawnPosition))
            {
                spawnedEnemy = PoolingManager.Instantiate(PoolingManager.Type.Enemy, PrefabsManager.GetPrefab(PrefabsManager.PrefabType.Enemy), randomSpawnPosition, Quaternion.identity);
                spawnedEnemy.GetComponent<Enemy>().speed = 4;
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
