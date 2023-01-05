using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Core.LevelManagement;
using UnityEngine.Events;

public class RestaurantManager : SingletonSceneMB<RestaurantManager> {
    public Transform door;
    public Level currentLevel;

    [SerializeReference]
    public Core.Words.Generator.ITextGenerator defaultGenerator;
    public UnityEvent OnCashierTriggered;
    public event Action<List<Core.Dish.DishItemSO>> OnPossibleDishesUpdated;
    
    private List<Core.Dish.DishItemSO> _possibleDishes;
    protected override void SingletonAwakened()
    {
        GlobalRef.I.CleanUpWords();
    }

    protected override void SingletonStarted()
    {
        GlobalRef.I.totalSales.Value = 0;
        GlobalRef.I.totalCustomerServed.Value = 0;
        GlobalRef.I.PlayBGM_Gameplay();
        WaveManager.I.StartWave();
        SetupLevel();
    }
    public List<Core.Dish.DishItemSO> GenerateDishes(int count)
    {
        List<Core.Dish.DishItemSO> result = new();
        for (var i = 0; i < count; i++) result.Add(_possibleDishes.GetRandom());
        return result;
    }
    private void UpdatePossibleDishes(IEnumerable<Core.Dish.DishItemSO> newPossibleDishes) {
        _possibleDishes = newPossibleDishes.ToList();
        OnPossibleDishesUpdated?.Invoke(_possibleDishes);
    }
    public void TriggerCashier()
    {
        OnCashierTriggered?.Invoke();
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
}
