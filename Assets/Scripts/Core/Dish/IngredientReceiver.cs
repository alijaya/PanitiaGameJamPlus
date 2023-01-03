using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Dish
{
    public abstract class IngredientReceiver : MonoBehaviour, IIngredientReceiver
    {
        public abstract bool IsBaseIngredient(IngredientItemSO ingredientItem);
        public abstract void AddIngredient(IngredientItemSO ingredientItem);
    }
}