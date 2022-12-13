using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineMaker : MonoBehaviour
{
    [SerializeField] private float outlineThickness;
    [SerializeField] private Transform outlineParent;
    [SerializeField] private List<GameObject> sprites;
    [SerializeField] private Material material;

    private void Awake()
    {
        foreach (GameObject sprite in sprites)
        {
            GameObject go = Instantiate(sprite, sprite.transform.position, sprite.transform.rotation, outlineParent);
            go.name = sprite.name + "_Outline";

            SpriteRenderer spriteRenderer = go.GetComponent<SpriteRenderer>();
            spriteRenderer.material = material;
            spriteRenderer.sortingOrder = 0;

            Vector3 scale = go.transform.localScale;
            go.transform.localScale = new Vector3(scale.x + outlineThickness, scale.y + outlineThickness, scale.z);
        }
    }
}
