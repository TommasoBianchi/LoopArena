using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    private Rigidbody2D myBody;
    private Player player;

    void Awake()
    {
        myBody = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<Player>();
    }

    void FixedUpdate()
    {
        Vector2 dirToPlayer = (Vector2)player.transform.position - myBody.position;
        myBody.velocity = dirToPlayer * speed;
    }
}
