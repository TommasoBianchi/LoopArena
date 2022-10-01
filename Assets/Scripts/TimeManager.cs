using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    public float resetEverySeconds = 10;
    private float timeToNextReset;
    private Stack<Snapshot> snapshots;

    void Start()
    {
        if (Instance != null)
        {
            throw new System.Exception("Impossible to have more than one TimeManager");
        }
        Instance = this;

        snapshots = new Stack<Snapshot>();

        timeToNextReset = resetEverySeconds;

        createSnapshot();
    }

    void Update()
    {
        timeToNextReset -= Time.deltaTime;

        if (timeToNextReset <= 0)
        {
            resetLastSnapshot();
            timeToNextReset = resetEverySeconds;
        }
    }

    private void resetLastSnapshot()
    {
        Snapshot lastSnapshot = snapshots.Peek();

        Player player = FindObjectOfType<Player>();
        player.transform.position = lastSnapshot.playerPosition;
        player.transform.rotation = lastSnapshot.playerRotation;
        // TODO: set player health

        // Set projectiles
        Projectile[] existingProjectiles = FindObjectsOfType<Projectile>(true);

        // If possible, reuse existing gameObjects
        for (int i = 0; i < Mathf.Min(existingProjectiles.Length, lastSnapshot.projectilePositions.Count); ++i)
        {
            existingProjectiles[i].transform.position = lastSnapshot.projectilePositions[i].Item1;
        }

        // Deactivate unnecessary existing gameObjects (do not destroy as we could reuse them later)
        for (int i = lastSnapshot.projectilePositions.Count; i < existingProjectiles.Length; ++i)
        {
            existingProjectiles[i].gameObject.SetActive(false);
        }

        // Instantiate extra projectiles (if needed)
        for (int i = existingProjectiles.Length; i < lastSnapshot.projectilePositions.Count; ++i)
        {
            Instantiate(PrefabsManager.GetPrefab(PrefabsManager.PrefabType.Projectile), lastSnapshot.projectilePositions[i].Item1, lastSnapshot.projectilePositions[i].Item2);
        }

        // Set enemies
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
