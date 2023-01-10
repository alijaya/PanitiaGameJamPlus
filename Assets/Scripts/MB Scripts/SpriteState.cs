using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteState : MonoBehaviour
{
    [SerializeField]
    private int _state = 0;

    public int State
    {
        get
        {
            return _state;
        }
        set
        {
            _state = value;
            RefreshUI();
        }
    }

    public int StateCount
    {
        get
        {
            return sprites.Count;
        }
    }

    public List<Sprite> sprites;

    public SpriteRenderer spriteRenderer;
    public Image image;

    private void OnEnable()
    {
        RefreshUI();
    }

    private void OnValidate()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        var s = Mathf.Clamp(_state, 0, sprites.Count - 1);
        var sprite = sprites.Count > 0 ? sprites[s] : null;

        if (spriteRenderer)
        {
            spriteRenderer.sprite = sprite;
        }

        if (image)
        {
            image.sprite = sprite;
        }
    }

    public void SetLast()
    {
        State = StateCount - 1;
    }

    public void SetFirst()
    {
        State = 0;
    }
}
