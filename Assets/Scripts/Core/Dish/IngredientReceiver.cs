using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Dish
{
    public abstract class IngredientReceiver : MonoBehaviour, IIngredientReceiver
    {
        protected HashSet<IngredientAdder> _adders = new ();

        public abstract bool IsBaseIngredient(IngredientItemSO ingredientItem);
        public abstract void AddIngredient(IngredientItemSO ingredientItem);

        public void RegisterAdder(IngredientAdder ingAdder)
        {
            _adders.Add(ingAdder);
        }

        public void UnregisterAdder(IngredientAdder ingAdder)
        {
            _adders.Remove(ingAdder);
        }
    }
}