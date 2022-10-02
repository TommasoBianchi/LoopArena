using UnityEngine;
public class Projectile : MonoBehaviour
{
    public float speed;
    public float lifetime;
    public Rect viewportDespawnRect;
    private Rigidbody2D myBody;

    private float remainingLifetime;

    private void OnEnable()
    {
        myBody = GetComponent<Rigidbody2D>();
        remainingLifetime = lifetime;
    }

    // TODO: collisions

    void Update()
    {
        myBody.velocity = transform.right * speed;

        remainingLifetime -= Time.deltaTime;

        if (remainingLifetime <= 0)
        {
            PoolingManager.Destroy(PoolingManager.Type.Projectile, gameObject);
            return;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        AudioManager.Play(AudioManager.ClipType.Hit);

        if (enemy != null)
        {
            enemy.ApplyDamage();
        }

        PoolingManager.Destroy(PoolingManager.Type.Projectile, gameObject);
        return;
    }
}
