using UnityEngine;

public class FreezePosition : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float lerpSpeed;

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, target.position + offset, lerpSpeed * Time.deltaTime);
    }
}
