using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using System;

public class CashierManager : SingletonMB<CashierManager>
{
    private HashSet<Cashier> cashiers = new();
    private Dictionary<Cashier, UnityAction<CustomerGroup>> cashierOccupiedHandlers = new();
    private Dictionary<Cashier, UnityAction<CustomerGroup>> cashierUnoccupiedHandlers = new();

    public UnityEvent<Cashier, CustomerGroup> OnCashierOccupied = new();
    public UnityEvent<Cashier, CustomerGroup> OnCashierUnoccupied = new();

    public void RegisterCashier(Cashier cashier)
    {
        if (cashiers.Add(cashier))
        {
            UnityAction<CustomerGroup> occupiedHandler = customerGroup =>
            {
                OnCashierOccupied.Invoke(cashier, customerGroup);
            };
            UnityAction<CustomerGroup> unoccupiedHandler = customerGroup =>
            {
                OnCashierUnoccupied.Invoke(cashier, customerGroup);
            };

            cashier.OnCashierOccupied.AddListener(occupiedHandler);
            cashier.OnCashierUnoccupied.AddListener(unoccupiedHandler);

            cashierOccupiedHandlers[cashier] = occupiedHandler;
            cashierUnoccupiedHandlers[cashier] = unoccupiedHandler;
        }
    }

    public void UnregisterCashier(Cashier cashier)
    {
        if (cashiers.Remove(cashier))
        {
            if (cashierOccupiedHandlers.TryGetValue(cashier, out var occupiedHandler))
            {
                cashier.OnCashierOccupied.RemoveListener(occupiedHandler);
                cashierOccupiedHandlers.Remove(cashier);
            }

            if (cashierUnoccupiedHandlers.TryGetValue(cashier, out var unoccupiedHandler))
            {
                cashier.OnCashierUnoccupied.RemoveListener(unoccupiedHandler);
                cashierUnoccupiedHandlers.Remove(cashier);
            }
        }
    }

    public Cashier GetAvailableCashier()
    {
        return cashiers.Where(cashier => cashier.CouldOccupy()).GetRandom();
    }

    public Cashier GetAvailableCashierQueue()
    {
        // should return null if no queue available, but rare ._.
        return cashiers.Aggregate((cashierA, cashierB) => cashierA.queueCount <= cashierB.queueCount ? cashierA : cashierB);
    }
}
