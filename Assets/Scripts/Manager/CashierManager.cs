using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using System;

public class CashierManager : SingletonSceneMB<CashierManager>
{
    private HashSet<Cashier> cashiers = new();
    private Dictionary<Cashier, UnityAction<CustomerGroup>> cashierQueuedHandlers = new();
    private Dictionary<Cashier, UnityAction<CustomerGroup>> cashierUnqueuedHandlers = new();

    public UnityEvent<Cashier, CustomerGroup> OnCashierQueued = new();
    public UnityEvent<Cashier, CustomerGroup> OnCashierUnqueued = new();

    public void RegisterCashier(Cashier cashier)
    {
        if (cashiers.Add(cashier))
        {
            UnityAction<CustomerGroup> queuedHandler = customerGroup =>
            {
                OnCashierQueued.Invoke(cashier, customerGroup);
            };
            UnityAction<CustomerGroup> unqueuedHandler = customerGroup =>
            {
                OnCashierUnqueued.Invoke(cashier, customerGroup);
            };

            cashier.OnCashierQueued.AddListener(queuedHandler);
            cashier.OnCashierUnqueued.AddListener(unqueuedHandler);

            cashierQueuedHandlers[cashier] = queuedHandler;
            cashierUnqueuedHandlers[cashier] = unqueuedHandler;
        }
    }

    public void UnregisterCashier(Cashier cashier)
    {
        if (cashiers.Remove(cashier))
        {
            if (cashierQueuedHandlers.TryGetValue(cashier, out var queuedHandler))
            {
                cashier.OnCashierQueued.RemoveListener(queuedHandler);
                cashierQueuedHandlers.Remove(cashier);
            }

            if (cashierUnqueuedHandlers.TryGetValue(cashier, out var unqueuedHandler))
            {
                cashier.OnCashierUnqueued.RemoveListener(unqueuedHandler);
                cashierUnqueuedHandlers.Remove(cashier);
            }
        }
    }

    public Cashier GetAvailableCashier()
    {
        return cashiers.Where(cashier => cashier.CouldOccupy()).GetRandom();
    }

    public Cashier GetAvailableCashierQueue()
    {
        var results = cashiers.Where(cashier => cashier.CouldQueue());

        if (results.Count() > 0) return results.Aggregate((cashierA, cashierB) => cashierA.queueCount <= cashierB.queueCount ? cashierA : cashierB);
        return null;
    }
}
