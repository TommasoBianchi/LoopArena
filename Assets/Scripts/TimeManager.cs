using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }
    public static float TimeFromStart { get; private set; }
    public static int NumStoredSnapshots { get { return Instance.snapshots.Count; } }

    public float resetEverySeconds = 10;
    public float timeToNextReset { get; private set; }
    private Stack<Snapshot> snapshots;
    private List<List<PlayerClone.ReplayStep>> playerPastTrajectories;

    void Start()
    {
        if (Instance != null)
        {
            throw new System.Exception("Impossible to have more than one TimeManager");
        }
        Instance = this;

        snapshots = new Stack<Snapshot>();
        playerPastTrajectories = new List<List<PlayerClone.ReplayStep>>();

        timeToNextReset = resetEverySeconds;
        TimeFromStart = 0;

        CreateSnapshot();
    }

    void Update()
    {
        timeToNextReset -= Time.deltaTime;

        if (timeToNextReset <= 0)
        {
            AudioManager.Play(AudioManager.ClipType.Reset);
            resetLastSnapshot();
        }
    }

    private void resetLastSnapshot()
    {
        Snapshot lastSnapshot = snapshots.Peek();

        // Restore global variables
        TimeFromStart = lastSnapshot.timeFromStart;
        FindObjectOfType<UIManager>().MonsterKilled = lastSnapshot.enemiesKilled;

        // Restore player data
        Player player = FindObjectOfType<Player>();
        player.transform.position = lastSnapshot.playerPosition;
        player.transform.rotation = lastSnapshot.playerRotation;
        player.currentHealth = lastSnapshot.playerHealth;

        // Store and reset player trajectory (only if the current checkpoints supports more clones)
        if (playerPastTrajectories.Count < Checkpoint.Current.maxClonesSupported)
        {
            playerPastTrajectories.Add(player.currentTrajectory);
        }
        player.ResetCurrentTrajectory();

        ///
        /// PLAYER CLONES
        ///
        PlayerClone[] existingPlayerClones = FindObjectsOfType<PlayerClone>();

        // Cleanup all already existing player clones
        for (int i = 0; i < existingPlayerClones.Length; ++i)
        {
            PoolingManager.Destroy(PoolingManager.Type.PlayerClone, existingPlayerClones[i].gameObject);
        }

        // Instantiate player clones (pooling whenever possible)
        for (int i = 0; i < playerPastTrajectories.Count; ++i)
        {
            GameObject playerClone = PoolingManager.Instantiate(
                PoolingManager.Type.PlayerClone,
                PrefabsManager.GetPrefab(PrefabsManager.PrefabType.PlayerClone),
                playerPastTrajectories[i][0].position, 
                Quaternion.identity
            );
            playerClone.GetComponent<PlayerClone>().SetTrajectory(playerPastTrajectories[i]);
        }

        ///
        /// PROJECTILES
        ///
        Projectile[] existingProjectiles = FindObjectsOfType<Projectile>();

        // Cleanup all already existing projectiles
        for (int i = 0; i < existingProjectiles.Length; ++i)
        {
            PoolingManager.Destroy(PoolingManager.Type.Projectile, existingProjectiles[i].gameObject);
        }

        // Instantiate projectiles (pooling whenever possible)
        for (int i = 0; i < lastSnapshot.projectilePositions.Count; ++i)
        {
            PoolingManager.Instantiate(
                PoolingManager.Type.Projectile,
                PrefabsManager.GetPrefab(PrefabsManager.PrefabType.Projectile),
                lastSnapshot.projectilePositions[i].Item1,
                lastSnapshot.projectilePositions[i].Item2
            );
        }

        ///
        /// ENEMIES
        ///
        Enemy[] existingEnemies = FindObjectsOfType<Enemy>();

        // Cleanup all already existing enemies
        for (int i = 0; i < existingEnemies.Length; ++i)
        {
            PoolingManager.Destroy(PoolingManager.Type.Enemy, existingEnemies[i].gameObject);
        }

        // Instantiate enemies (pooling whenever possible)
        for (int i = 0; i < lastSnapshot.enemyData.Count; ++i)
        {
            GameObject enemy = PoolingManager.Instantiate(
                PoolingManager.Type.Enemy,
                PrefabsManager.GetPrefab(PrefabsManager.PrefabType.Enemy),
                lastSnapshot.enemyData[i].position,
                lastSnapshot.enemyData[i].rotation
            );
            enemy.GetComponent<Enemy>().health = lastSnapshot.enemyData[i].health;
        }

        // Consume durability from the current checkpoint
        Checkpoint.Current.DecreaseDurability();

        timeToNextReset = resetEverySeconds;
    }

    public void CreateSnapshot()
    {
        // Cleanup (future) player clones
        playerPastTrajectories.Clear();

        // Cleanup all already existing player clones
        PlayerClone[] existingPlayerClones = FindObjectsOfType<PlayerClone>();
        for (int i = 0; i < existingPlayerClones.Length; ++i)
        {
            PoolingManager.Destroy(PoolingManager.Type.PlayerClone, existingPlayerClones[i].gameObject);
        }

        Player player = FindObjectOfType<Player>();

        Projectile[] projectiles = FindObjectsOfType<Projectile>();
        Enemy[] enemies = FindObjectsOfType<Enemy>();

        Snapshot snapshot = new Snapshot(
            TimeFromStart,
            FindObjectOfType<UIManager>().MonsterKilled,
            player.transform.position,
            player.transform.rotation,
            player.currentHealth,
            projectiles.Select(p => ((Vector2)p.transform.position, p.transform.rotation)).ToList(),
            enemies.Select(e => new EnemySnapshotData(e.transform.position, e.transform.rotation, e.health)).ToList()
        );

        snapshots.Push(snapshot);

        timeToNextReset = resetEverySeconds;
    }

    public void RollbackSnapshot()
    {
        // Cleanup player clones
        playerPastTrajectories.Clear();

        snapshots.Pop();
        resetLastSnapshot();
    }

    struct Snapshot
    {
        public float timeFromStart;
        public int enemiesKilled;
        public Vector2 playerPosition;
        public Quaternion playerRotation;
        public float playerHealth;
        public List<(Vector2, Quaternion)> projectilePositions;
        public List<EnemySnapshotData> enemyData;

        public Snapshot(
            float timeFromStart,
            int enemiesKilled,
            Vector2 playerPosition, 
            Quaternion playerRotation, 
            float playerHealth, List<(Vector2, Quaternion)> projectilePositions, 
            List<EnemySnapshotData> enemyData
        )
        {
            this.timeFromStart = timeFromStart;
            this.enemiesKilled = enemiesKilled;
            this.playerPosition = playerPosition;
            this.playerRotation = playerRotation;
            this.playerHealth = playerHealth;
            this.projectilePositions = projectilePositions;
            this.enemyData = enemyData;
        }
    }

    struct EnemySnapshotData
    {
        public Vector2 position;
        public Quaternion rotation;
        public float health;

        public EnemySnapshotData(Vector2 position, Quaternion rotation, float health)
        {
            this.position = position;
            this.rotation = rotation;
            this.health = health;
        }
    }
}
