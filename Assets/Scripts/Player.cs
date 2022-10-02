using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform rotationAnchor;
    public Transform projectileSpawnPoint;
    public float speed;
    public float health;
    public float currentHealth;
    public float invulnerabilityDuration;
    public float knockbackDuration;
    public float knockbackStrength;
    private Rigidbody2D myBody;
    private Animator animatorController;
    private float invulnerabilityRemainingTime;
    private float knockbackRemainingTime;
    private Vector2 knockbackDirection;

    public List<PlayerClone.ReplayStep> currentTrajectory { get; private set; }

    void Start()
    {
        myBody = GetComponent<Rigidbody2D>();
        animatorController = GetComponentInChildren<Animator>();
        currentTrajectory = new List<PlayerClone.ReplayStep>();
        currentHealth = health;
    }

    public void ResetCurrentTrajectory()
    {
        currentTrajectory = new List<PlayerClone.ReplayStep>();
    }

    void Update()
    {
        // Decrease invulnerability timer
        if (invulnerabilityRemainingTime > 0)
        {
            invulnerabilityRemainingTime -= Time.deltaTime;
        }

        // Decrease knockback timer
        if (knockbackRemainingTime > 0)
        {
            knockbackRemainingTime -= Time.deltaTime;
        }

        // Move
        Vector2 moveDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
        if (knockbackRemainingTime > 0)
        {
            // Apply knockback
            moveDirection = Vector2.Lerp(moveDirection, knockbackDirection * knockbackStrength, knockbackRemainingTime / knockbackDuration);
        }
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

    public void ApplyDamage(Vector2 contactNormal)
    {
        if (invulnerabilityRemainingTime > 0)
        {
            // No damage taken
            return;
        }

        currentHealth--;

        invulnerabilityRemainingTime = invulnerabilityDuration;
        knockbackRemainingTime = knockbackDuration;

        // Knockback
        knockbackDirection = -contactNormal;

        if (currentHealth <= 0)
        {
            UIManager.GameOver(false);
        }
    }
}
