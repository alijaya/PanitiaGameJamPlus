using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Core.Dish;
using Core.LevelManagement;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class RestaurantManager : SingletonSceneMB<RestaurantManager> {
    public event Action<List<DishItemSO>> OnPossibleDishesUpdated;
    
    public Transform door;
    [InlineEditor]
    public Level currentLevel;
    public IngredientReceiver[] ingredientReceivers;

    [SerializeReference]
    public Core.Words.Generator.ITextGenerator defaultGenerator;
    public UnityEvent OnCashierTriggered;

    public ScorebarUI scorebarUI;

    private List<DishItemSO> _possibleDishes;

    protected override void SingletonAwakened()
    {
        
    }

    protected override void SingletonStarted()
    {
        SetupLevel();
    }

    private void UpdatePossibleDishes(IEnumerable<DishItemSO> newPossibleDishes) {
        _possibleDishes = newPossibleDishes.ToList();
        OnPossibleDishesUpdated?.Invoke(_possibleDishes);
    }

    private void SetupLevel() {
        if (currentLevel == null) return;
        GlobalRef.I.totalSales.Value = 0;
        GlobalRef.I.totalCustomerServed.Value = 0;

        if (scorebarUI)
        {
            scorebarUI.Goals = currentLevel.goals;
            scorebarUI.MaxGoal = currentLevel.maxGoal;
        }
        UpdatePossibleDishes(currentLevel.possibleDish);

        GlobalRef.I.PlayBGM_Gameplay();
        WaveManager.I.StartWave();
        // setup tray slot
        // setup shift duration
    }

    private void SetupNewLevel(Level newLevel) {
        currentLevel = newLevel;
        SetupLevel();
    }

    private void Update() {
        // Testing to move to level selection scene 
        if (Input.GetKeyDown(KeyCode.Escape)) {
            GlobalRef.I.GoToScene_LevelSelection();
        }
    }

    public void HandleCustomerPay(CustomerGroup customerGroup)
    {
        GlobalRef.I.totalSales.Value += customerGroup.totalDishPrice;
        GlobalRef.I.totalCustomerServed.Value ++;
    }

    public void TriggerCashier()
    {
        OnCashierTriggered?.Invoke();
    }

    public void RegisterAdder(IngredientAdder adder) {
        var receiver = ingredientReceivers.FirstOrDefault(x => x.IsValidIngredient(adder.GetIngredient()));
        if (receiver) {
            receiver.RegisterAdder(adder);
            adder.IsBaseIngredient = receiver.IsBaseIngredient(adder.GetIngredient());
            
            //_addersLookup[receiver].Add(adder);
        }
        
    }

    public bool ValidateItem(TrayItemSO trayItem) {
        switch (trayItem) {
            case IngredientItemSO ingredientItem:
                var receiver = ingredientReceivers.FirstOrDefault(x => x.IsValidIngredient(ingredientItem));
                if (!receiver) return false;
                receiver.AddIngredient(ingredientItem);
                return true;
            case DishItemSO dishItem:
                return Tray.I.AddDish(dishItem);
        }

        return false;
    }

    public void ValidateItemRecipe(TrayItemSO itemSo) {
        ValidateItem(itemSo);
    }

    public List<DishItemSO> GenerateDishes(int count)
    {
        List<Core.Dish.DishItemSO> result = new();
        for (var i = 0; i < count; i++) result.Add(_possibleDishes.GetRandom());
        return result;
    }
}
