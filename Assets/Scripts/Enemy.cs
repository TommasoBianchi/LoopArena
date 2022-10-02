using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    public float speed;
    public float health;

    private Animator animatorController;
    private NavMeshAgent navMeshAgent;
    private Player player;
    private UIManager UI;

    void Awake()
    {
        animatorController = GetComponentInChildren<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        player = FindObjectOfType<Player>();
        UI = FindObjectOfType<UIManager>();

        navMeshAgent.speed = speed;
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
    }

    void FixedUpdate()
    {
        navMeshAgent.SetDestination(player.transform.position);

        // Update the animation controller
        Vector2 lookDirection = player.transform.position - transform.position;
        int animationDirection;
        if (Mathf.Abs(lookDirection.x) > Mathf.Abs(lookDirection.y))
        {
            // Going sideways (either right if x > 0 or left if x < 0
            animationDirection = lookDirection.x > 0 ? 1 : 3;
        }
        else
        {
            // Going up (y > 0) or down (y < 0)
            animationDirection = lookDirection.y > 0 ? 2 : 0;
        }

        bool isMoving = navMeshAgent.velocity.sqrMagnitude > 0.01f;

        animatorController.SetInteger("direction", animationDirection);
        animatorController.SetBool("is_moving", isMoving);
    }

    public void ApplyDamage()
    {
        health--;

        if (health <= 0)
        {
            PoolingManager.Destroy(PoolingManager.Type.Enemy, gameObject);
            AudioManager.Play(AudioManager.ClipType.MonsterDeath);
            UI.Kill();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();

        if (player != null)
        {
            player.ApplyDamage();
        }

        return;
    }
}
