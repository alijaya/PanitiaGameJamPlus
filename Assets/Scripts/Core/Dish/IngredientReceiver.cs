using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Dish
{
    public abstract class IngredientReceiver : MonoBehaviour, IIngredientReceiver
    {
        protected readonly HashSet<IngredientAdder> adders = new ();
        public abstract bool IsBaseIngredient(IngredientItemSO ingredientItem);
        public abstract bool IsValidIngredient(IngredientItemSO ingredientItem);
        public abstract void AddIngredient(IngredientItemSO ingredientItem);

        public void RegisterAdder(IngredientAdder ingAdder)
        {
            adders.Add(ingAdder);
        }

        public void UnregisterAdder(IngredientAdder ingAdder)
        {
            adders.Remove(ingAdder);
        }
    }
}