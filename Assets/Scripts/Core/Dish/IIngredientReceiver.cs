namespace Core.Dish {
    public interface IIngredientReceiver {
        void AddIngredient(IngredientItemSO ingredientItem);
        bool IsBaseIngredient(IngredientItemSO ingredientItem);
    }
}