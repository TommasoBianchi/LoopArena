using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    public float resetEverySeconds = 10;
    private float timeToNextReset;
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

        createSnapshot();
    }

    void Update()
    {
        timeToNextReset -= Time.deltaTime;

        if (timeToNextReset <= 0)
        {
            resetLastSnapshot();
        }

        // TEMPORARY
        if (Input.GetKeyDown(KeyCode.Space))
        {
            createSnapshot();
            // TODO: destroy player clones
        }
    }

    private void resetLastSnapshot()
    {
        Snapshot lastSnapshot = snapshots.Peek();

        // Store player data
        Player player = FindObjectOfType<Player>();
        player.transform.position = lastSnapshot.playerPosition;
        player.transform.rotation = lastSnapshot.playerRotation;
        // TODO: set player health

        // Store and reset player trajectory
        playerPastTrajectories.Add(player.currentTrajectory);
        player.ResetCurrentTrajectory();

        // Set player clones
        PlayerClone[] existingPlayerClones = FindObjectsOfType<PlayerClone>(true);

        // If possible, reuse existing gameObjects
        for (int i = 0; i < Mathf.Min(existingPlayerClones.Length, playerPastTrajectories.Count); ++i)
        {
            existingPlayerClones[i].SetTrajectory(playerPastTrajectories[i]);
        }

        // Instantiate extra player clone (should be exactly one)
        for (int i = existingPlayerClones.Length; i < playerPastTrajectories.Count; ++i)
        {
            GameObject playerClone = PoolingManager.Instantiate(
                PoolingManager.Type.PlayerClone,
                PrefabsManager.GetPrefab(PrefabsManager.PrefabType.PlayerClone),
                playerPastTrajectories[i][0].position, 
                playerPastTrajectories[i][0].rotation
            );
            playerClone.GetComponent<PlayerClone>().SetTrajectory(playerPastTrajectories[i]);
        }

        // Set projectiles
        Projectile[] existingProjectiles = FindObjectsOfType<Projectile>(true);

        // If possible, reuse existing gameObjects
        for (int i = 0; i < Mathf.Min(existingProjectiles.Length, lastSnapshot.projectilePositions.Count); ++i)
        {
            existingProjectiles[i].transform.position = lastSnapshot.projectilePositions[i].Item1;
        }

        // Deactivate unnecessary existing gameObjects (use PoolingManager to reuse them later)
        for (int i = lastSnapshot.projectilePositions.Count; i < existingProjectiles.Length; ++i)
        {
            PoolingManager.Destroy(PoolingManager.Type.Projectile, existingProjectiles[i].gameObject);
        }

        // Instantiate extra projectiles (if needed)
        for (int i = existingProjectiles.Length; i < lastSnapshot.projectilePositions.Count; ++i)
        {
            PoolingManager.Instantiate(
                PoolingManager.Type.Projectile, 
                PrefabsManager.GetPrefab(PrefabsManager.PrefabType.Projectile), 
                lastSnapshot.projectilePositions[i].Item1, 
                lastSnapshot.projectilePositions[i].Item2
            );
        }

        // TODO: set enemies

        timeToNextReset = resetEverySeconds;
    }

    private void createSnapshot()
    {
        Player player = FindObjectOfType<Player>();

        Projectile[] projectiles = FindObjectsOfType<Projectile>();

        Snapshot snapshot = new Snapshot(
            player.transform.position,
            player.transform.rotation,
            1, 
            projectiles.Select(p => ((Vector2)p.transform.position, p.transform.rotation)).ToList(), 
            new List<(Vector2, Quaternion)>()
        );

        snapshots.Push(snapshot);

        timeToNextReset = resetEverySeconds;
    }

    struct Snapshot
    {
        public Vector2 playerPosition;
        public Quaternion playerRotation;
        public float playerHealth;
        public List<(Vector2, Quaternion)> projectilePositions;
        public List<(Vector2, Quaternion)> enemyPositions;

        public Snapshot(Vector2 playerPosition, Quaternion playerRotation, float playerHealth, List<(Vector2, Quaternion)> projectilePositions, List<(Vector2, Quaternion)> enemyPositions)
        {
            this.playerPosition = playerPosition;
            this.playerRotation = playerRotation;
            this.playerHealth = playerHealth;
            this.projectilePositions = projectilePositions;
            this.enemyPositions = enemyPositions;
        }
    }
}
