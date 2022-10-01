using UnityEngine;
public class Projectile : MonoBehaviour
{
    public float speed;
    public float lifetime;
    public Rect viewportDespawnRect;

    // TODO: collisions

    void Update()
    {
        transform.Translate(transform.right * speed * Time.deltaTime, Space.World);

        lifetime -= Time.deltaTime;

        if (lifetime <= 0)
        {
            Destroy(gameObject);
        }

        // Despawn if out of camera view
        Vector2 cameraViewport = Camera.main.WorldToViewportPoint(transform.position);

        if (!viewportDespawnRect.Contains(cameraViewport))
        {
            Destroy(gameObject);
        }
    }
}
