using System.Collections.Generic;
using UnityEngine;

public class PlayerClone : MonoBehaviour
{
    public Transform projectileSpawnPoint;

    private List<ReplayStep> trajectory;
    private int step = 0;

    public void SetTrajectory(List<ReplayStep> trajectory)
    {
        this.trajectory = trajectory;
        step = 0;

        transform.position = trajectory[0].position;
        transform.rotation = trajectory[0].rotation;
    }

    void Update()
    {
        if (trajectory == null || trajectory.Count <= step)
        {
            return;
        }

        ReplayStep replayStep = trajectory[step];
        ++step;

        transform.position = replayStep.position;
        transform.rotation = replayStep.rotation;

        // Shoot
        if (replayStep.hasFiredProjectile)
        {
            PoolingManager.Instantiate(
                PoolingManager.Type.Projectile, 
                PrefabsManager.GetPrefab(PrefabsManager.PrefabType.Projectile), 
                projectileSpawnPoint.position, 
                transform.rotation
            );
        }
    }

    public struct ReplayStep
    {
        public Vector2 position;
        public Quaternion rotation;
        public bool hasFiredProjectile;

        public ReplayStep(Vector2 position, Quaternion rotation, bool hasFiredProjectile)
        {
            this.position = position;
            this.rotation = rotation;
            this.hasFiredProjectile = hasFiredProjectile;
        }
    }
}
