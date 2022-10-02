using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    public float speed;
    public float health;

    private NavMeshAgent navMeshAgent;
    private Player player;

    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        player = FindObjectOfType<Player>();

        navMeshAgent.speed = speed;
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
    }

    void FixedUpdate()
    {
        navMeshAgent.SetDestination(player.transform.position);
    }

    public void ApplyDamage()
    {
        health--;

        if (health <= 0)
        {
            PoolingManager.Destroy(PoolingManager.Type.Enemy, gameObject);
            AudioManager.Play(AudioManager.ClipType.MonsterDeath);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();

        if (player != null)
        {
            Time.timeScale = 0;
        }

        return;
    }
}
