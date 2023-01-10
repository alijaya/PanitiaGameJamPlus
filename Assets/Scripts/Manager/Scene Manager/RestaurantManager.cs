using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Core.Dish;
using Core.LevelManagement;
using UnityEngine.Events;

public class RestaurantManager : SingletonSceneMB<RestaurantManager> {
    public event Action<List<DishItemSO>> OnPossibleDishesUpdated;
    
    public Transform door;
    public Level currentLevel;
    public IngredientReceiver[] ingredientReceivers;

    [SerializeReference]
    public Core.Words.Generator.ITextGenerator defaultGenerator;
    public UnityEvent OnCashierTriggered;

    private List<DishItemSO> _possibleDishes;

    private readonly Dictionary<IngredientReceiver, List<IngredientAdder>> _addersLookup = new();

    protected override void SingletonAwakened()
    {
        
    }

    protected override void SingletonStarted()
    {
        GlobalRef.I.totalSales.Value = 0;
        GlobalRef.I.totalCustomerServed.Value = 0;
        GlobalRef.I.PlayBGM_Gameplay();
        WaveManager.I.StartWave();
        SetupLevel();
    }

    private void UpdatePossibleDishes(IEnumerable<DishItemSO> newPossibleDishes) {
        _possibleDishes = newPossibleDishes.ToList();
        OnPossibleDishesUpdated?.Invoke(_possibleDishes);
    }

    private void SetupLevel() {
        if (currentLevel == null) return;
        UpdatePossibleDishes(currentLevel.possibleDish);
        // setup tray slot
        // setup shift duration
        // setup goal threshold
    }

    private void SetupNewLevel(Level newLevel) {
        currentLevel = newLevel;
        SetupLevel();
    }

    private void OnValidate() {
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
