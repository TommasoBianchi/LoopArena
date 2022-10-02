using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    public float speed;
    public float health;

    private NavMeshAgent navMeshAgent;
    private Player player;
    private UIManager UI;

    void Awake()
    {
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
    }

    public void ApplyDamage()
    {
        health--;

        if (health <= 0)
        {
            PoolingManager.Destroy(PoolingManager.Type.Enemy, gameObject);
            AudioManager.Play(AudioManager.ClipType.MonsterDeath);
            UI.kill();
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
