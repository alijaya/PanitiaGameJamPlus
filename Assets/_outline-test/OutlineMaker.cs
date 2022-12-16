using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class OutlineMaker : MonoBehaviour
{
    [SerializeField] private float outlineThickness;
    [SerializeField] private Transform outlineParent;
    [SerializeField] private List<GameObject> sprites;
    [SerializeField] private Material material;

    public List<GameObject> _outlines;

    private void Awake()
    {
        if (outlineParent.childCount == 0) InstantiateOutlines();
    }
    
    private void InstantiateOutlines()
    {
        if (_outlines.Count >= 1) return;
        
        DeleteOutlines();
        
        foreach (GameObject sprite in sprites)
        {
            GameObject go = Instantiate(sprite, sprite.transform.position, sprite.transform.rotation, outlineParent);
            go.name = sprite.name + "_Outline";

            SpriteRenderer spriteRenderer = go.GetComponent<SpriteRenderer>();
            spriteRenderer.material = material;
            spriteRenderer.sortingOrder = 0;

            Vector3 scale = go.transform.localScale;
            go.transform.localScale = new Vector3(scale.x + outlineThickness, scale.y + outlineThickness, scale.z);
            
            _outlines.Add(go);
        }
    }
    
    [PropertySpace]

    [Button("Generate Outlines")]
    private void InstantiateOutlinesEditor()
    {
        InstantiateOutlines();
    }

    [Button("Delete Outlines")]
    private void DeleteOutlines()
    {
        foreach (GameObject outline in _outlines)
        {
            DestroyImmediate(outline);    
        }

        _outlines.Clear();
    }
}
