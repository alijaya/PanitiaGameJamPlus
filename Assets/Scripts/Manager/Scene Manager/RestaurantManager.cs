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

    public UnityEvent OnCashierTriggered;

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
    }

    public List<Core.Dish.DishItemSO> GenerateDishes(int count)
    {
        List<Core.Dish.DishItemSO> result = new();
        for (var i = 0; i < count; i++) result.Add(possibleDishes.GetRandom());
        return result;
    }

    public void TriggerCashier()
    {
        OnCashierTriggered?.Invoke();
    }
}
