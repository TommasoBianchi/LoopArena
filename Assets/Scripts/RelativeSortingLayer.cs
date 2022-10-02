using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class RelativeSortingLayer : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    public SpriteRenderer parentSpriteRenderer;


    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        spriteRenderer.sortingOrder = parentSpriteRenderer.sortingOrder +1;
    }
}
