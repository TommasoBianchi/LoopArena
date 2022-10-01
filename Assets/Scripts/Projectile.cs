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

        // Despawn if out of camera view
        Vector2 cameraViewport = Camera.main.WorldToViewportPoint(transform.position);

        if (!viewportDespawnRect.Contains(cameraViewport))
        {
            PoolingManager.Destroy(PoolingManager.Type.Projectile, gameObject);
            return;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PoolingManager.Destroy(PoolingManager.Type.Enemy, collision.gameObject.GetComponent<Enemy>().gameObject);
        PoolingManager.Destroy(PoolingManager.Type.Projectile, gameObject);
        return;
    }
}
