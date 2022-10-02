using System.Collections.Generic;
using UnityEngine;

public class PlayerClone : MonoBehaviour
{
    public Transform rotationAnchor;
    public Transform projectileSpawnPoint;

    private Animator animatorController;
    private List<ReplayStep> trajectory;
    private int step = 0;

    private void Start()
    {
        animatorController = GetComponentInChildren<Animator>();
    }

    public void SetTrajectory(List<ReplayStep> trajectory)
    {
        this.trajectory = trajectory;
        step = 0;

        transform.position = trajectory[0].position;
        rotationAnchor.rotation = trajectory[0].rotation;
    }

    void Update()
    {
        if (trajectory == null || trajectory.Count <= step)
        {
            return;
        }

        ReplayStep replayStep = trajectory[step];
        ++step;

        bool isMoving = (replayStep.position - (Vector2)transform.position).sqrMagnitude < 0.01f;
        transform.position = replayStep.position;
        rotationAnchor.rotation = replayStep.rotation;

        // Shoot
        if (replayStep.hasFiredProjectile)
        {
            PoolingManager.Instantiate(
                PoolingManager.Type.Projectile, 
                PrefabsManager.GetPrefab(PrefabsManager.PrefabType.Projectile), 
                projectileSpawnPoint.position,
                rotationAnchor.rotation
            );

            AudioManager.Play(AudioManager.ClipType.Shoot);
        }

        animatorController.SetInteger("direction", replayStep.animatorDirection);
        animatorController.SetBool("is_moving", isMoving);
    }

    public struct ReplayStep
    {
        public Vector2 position;
        public Quaternion rotation;
        public bool hasFiredProjectile;
        public int animatorDirection;
        public bool isMoving;

        public ReplayStep(Vector2 position, Quaternion rotation, bool hasFiredProjectile, int animatorDirection, bool isMoving)
        {
            this.position = position;
            this.rotation = rotation;
            this.hasFiredProjectile = hasFiredProjectile;
            this.animatorDirection = animatorDirection;
            this.isMoving = isMoving;
        }
    }
}
