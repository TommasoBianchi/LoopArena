using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform rotationAnchor;
    public Transform projectileSpawnPoint;
    public float speed;
    public float health;
    private Rigidbody2D myBody;
    private Animator animatorController;

    public List<PlayerClone.ReplayStep> currentTrajectory { get; private set; }

    void Start()
    {
        health = 10;
        myBody = GetComponent<Rigidbody2D>();
        animatorController = GetComponentInChildren<Animator>();
        currentTrajectory = new List<PlayerClone.ReplayStep>();
    }

    public void ResetCurrentTrajectory()
    {
        currentTrajectory = new List<PlayerClone.ReplayStep>();
    }

    void Update()
    {
        // Move
        Vector2 moveDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
        myBody.velocity = moveDirection * speed;

        // Look towards the mouse cursor
        Vector2 lookDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;

        if (lookDirection.sqrMagnitude > 0.01f)
        {
            rotationAnchor.rotation = Quaternion.Euler(0, 0, -Vector3.SignedAngle(lookDirection, Vector3.right, Vector3.forward));
        }

        // Shoot
        if (Input.GetMouseButtonDown(0))
        {
            PoolingManager.Instantiate(
                PoolingManager.Type.Projectile,
                PrefabsManager.GetPrefab(PrefabsManager.PrefabType.Projectile), 
                projectileSpawnPoint.position,
                rotationAnchor.rotation
            );

            AudioManager.Play(AudioManager.ClipType.Shoot);
        }

        // Update the animation controller
        int animationDirection;
        if (Mathf.Abs(lookDirection.x) > Mathf.Abs(lookDirection.y))
        {
            // Going sideways (either right if x > 0 or left if x < 0
            animationDirection = lookDirection.x > 0 ? 1 : 3;
        } else
        {
            // Going up (y > 0) or down (y < 0)
            animationDirection = lookDirection.y > 0 ? 2 : 0;
        }

        bool isMoving = moveDirection.sqrMagnitude > 0.01f;

        animatorController.SetInteger("direction", animationDirection);
        animatorController.SetBool("is_moving", isMoving);

        // Save current step in trajectory
        currentTrajectory.Add(new PlayerClone.ReplayStep(
            transform.position,
            rotationAnchor.rotation,
            Input.GetMouseButtonDown(0),
            animationDirection,
            isMoving
        ));
    }

    public void ApplyDamage()
    {
        health--;

        if (health <= 0)
        {
            Debug.Log("Sei morto!");
            Time.timeScale = 0;
        }
    }
}
