using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityAtoms.BaseAtoms;
using UnityEngine.Events;

public class RestaurantManager : SingletonSceneMB<RestaurantManager>
{
    public Transform door;

    public List<Core.Dish.DishItemSO> possibleDishes;

    [SerializeReference]
    public Core.Words.Generator.ITextGenerator defaultGenerator;

    public UnityEvent OnCashierTriggered;
    //public UnityEvent<List<Core.Dish.DishItemSO>> onPossibleDishesUpdated;

    public event Action<List<Core.Dish.DishItemSO>> OnPossibleDishesUpdated;

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
        UpdatePossibleDishes(GlobalRef.I.LevelProgression.GetCurrentLevel().possibleDish);
    }

    public List<Core.Dish.DishItemSO> GenerateDishes(int count)
    {
        List<Core.Dish.DishItemSO> result = new();
        for (var i = 0; i < count; i++) result.Add(possibleDishes.GetRandom());
        return result;
    }

    public void UpdatePossibleDishes(IEnumerable<Core.Dish.DishItemSO> newPossibleDishes) {
        possibleDishes = newPossibleDishes.ToList();
        OnPossibleDishesUpdated?.Invoke(possibleDishes);
    }

    public void TriggerCashier()
    {
        OnCashierTriggered?.Invoke();
    }

    private void OnValidate() {
        OnPossibleDishesUpdated?.Invoke(possibleDishes);
    }

    private void Update() {
        // Testing to move to level selection scene 
        if (Input.GetKeyDown(KeyCode.Escape)) {
            GlobalRef.I.GoToScene_LevelSelection();
        }
    }
}
