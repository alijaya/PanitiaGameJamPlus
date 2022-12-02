using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu]
public class CustomerTypeSO : ScriptableObject
{
    public List<Sprite> sprites;

    // this is relative value, 1 is normal, 0.5 is half the normal, 2 is double the normal
    public float patience = 1;
    public float walkSpeed = 1;
    public float orderDuration = 1;
    public float eatDuration = 1;
    public float bonusTip = 1;
    [MinMaxSlider(0, 4, true)]
    public Vector2 foodDifficulty = Vector2.one;
    [MinMaxSlider(0, 4, true)]
    public Vector2 wordDifficulty = Vector2.one;
}
