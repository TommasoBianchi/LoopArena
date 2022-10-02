using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SortingLayerUpdater : MonoBehaviour
{
    public bool updateAtRuntime;
    public int offset;

    private SpriteRenderer spriteRenderer;
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        updateSortingOrder();
    }

    void Update()
    {
        if (updateAtRuntime)
        {
            updateSortingOrder();
        }
    }

    private void OnValidate()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        updateSortingOrder();
    }

    private void updateSortingOrder()
    {
        spriteRenderer.sortingOrder = Mathf.FloorToInt(-100 * transform.position.y + offset);
    }
}
