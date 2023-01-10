using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu]
public class CustomerTypeSO : ScriptableObject
{
    [AssetSelector]
    public List<Customer> prefabs;

    // this is relative value, 1 is normal, 0.5 is half the normal, 2 is double the normal
    public float walkSpeed = 1;
    public float waitDuration = 1;
    public float orderDuration = 1;
    public float eatDuration = 1;
    public float bonusTip = 1;
    [MinMaxSlider(0, 4, true)]
    public Vector2 foodDifficulty = Vector2.one;
    [MinMaxSlider(0, 4, true)]
    public Vector2 wordDifficulty = Vector2.one;

    [FoldoutGroup("Text Generation")]
    [SerializeReference]
    public Core.Words.Generator.ITextGenerator TextGenerator;

    [FoldoutGroup("Text Generation")]
    [SerializeReference]
    public List<Core.Words.Modifier.ITextModifier> TextModifiers = new();


    // in unit / seconds
    public float WalkSpeed
    {
        get
        {
            return walkSpeed * GameBalance.I.walkSpeed;
        }
    }

    // in seconds
    public float WaitDuration
    {
        get
        {
            return waitDuration * GameBalance.I.waitDuration;
        }
    }

    // in seconds
    public float OrderDuration
    {
        get
        {
            return orderDuration * GameBalance.I.orderDuration;
        }
    }

    // in seconds
    public float EatDuration
    {
        get
        {
            return eatDuration * GameBalance.I.eatDuration;
        }
    }

    // in money
    public float BonusTip
    {
        get
        {
            return bonusTip * GameBalance.I.bonusTip;
        }
    }

    // in Vector2
    public Vector2 FoodDifficulty
    {
        get
        {
            return foodDifficulty * GameBalance.I.foodDifficulty;
        }
    }

    // in Vector2
    public Vector2 WordDifficulty
    {
        get
        {
            return wordDifficulty * GameBalance.I.wordDifficulty;
        }
    }

}
