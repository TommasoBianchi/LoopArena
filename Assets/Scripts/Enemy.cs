using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    public float health;
    private Rigidbody2D myBody;
    private Player player;

    void Awake()
    {
        myBody = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<Player>();
    }

    void FixedUpdate()
    {
        Vector2 dirToPlayer = ((Vector2)player.transform.position - myBody.position).normalized;
        myBody.velocity = dirToPlayer * speed;
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
