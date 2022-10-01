using UnityEngine;
public class Projectile : MonoBehaviour
{
    public float speed;
    public float lifetime;
    public Rect viewportDespawnRect;

    private float remainingLifetime;

    private void OnEnable()
    {
        remainingLifetime = lifetime;
    }

    // TODO: collisions

    void Update()
    {
        transform.Translate(transform.right * speed * Time.deltaTime, Space.World);

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
}
