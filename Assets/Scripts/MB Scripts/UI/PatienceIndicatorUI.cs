using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatienceIndicatorUI : MonoBehaviour
{
    [SerializeField]
    [Range(0,1)]
    private float _value;

    public float Value
    {
        get
        {
            return _value;
        }
        set
        {
            _value = value;
            RefreshUI();
        }
    }

    public List<SpriteState> spriteStates = new ();

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
        var index = Mathf.CeilToInt(_value * spriteStates.Count) - 1;

        for (var i = 0; i < spriteStates.Count; i++)
        {
            var spriteState = spriteStates[i];

            if (i < index)
            {
                spriteState.SetLast();
            } else if (i > index)
            {
                spriteState.SetFirst();
            } else
            {
                var fract = _value * spriteStates.Count - index; // 0 - 1

                var state = Mathf.CeilToInt(fract * (spriteState.StateCount - 1));

                spriteState.State = state;
            }
        }
    }
}
