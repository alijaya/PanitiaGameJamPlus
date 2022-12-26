using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Cashier : MonoBehaviour
{
    public List<CustomerGroup> queueCustomerGroups { get; private set; } = new();

    public List<PointOfInterest> queueLocations;

    public UnityEvent<CustomerGroup> OnCashierQueued = new();
    public UnityEvent<CustomerGroup> OnCashierUnqueued = new();

    public CustomerGroup customerGroup
    {
        get
        {
            if (queueCustomerGroups.Count > 0) return queueCustomerGroups[0];
            return null;
        }
    }

    public PointOfInterest location
    {
        get
        {
            if (queueLocations.Count > 0) return queueLocations[0];
            return null;
        }
    }

    public bool occupied
    {
        get
        {
            return queueCustomerGroups.Count > 0;
        }
    }

    public int queueCount
    {
        get
        {
            return queueCustomerGroups.Count;
        }
    }

    private void OnEnable()
    {
        CashierManager.I.RegisterCashier(this);
    }

    private void OnDisable()
    {
        CashierManager.I.UnregisterCashier(this);
    }

    public int QueueCustomerGroup(CustomerGroup customerGroup)
    {
        if (!this.queueCustomerGroups.Contains(customerGroup))
        {
            this.queueCustomerGroups.Add(customerGroup);
            OnCashierQueued.Invoke(customerGroup);
            return this.queueCustomerGroups.Count - 1;
        } else
        {
            return this.queueCustomerGroups.IndexOf(customerGroup);
        }
    }

    public void UnqueueCustomerGroup(CustomerGroup customerGroup)
    {
        if (this.queueCustomerGroups.Contains(customerGroup))
        {
            this.queueCustomerGroups.Remove(customerGroup);
            OnCashierUnqueued.Invoke(customerGroup);
        }
    }

    public bool CouldOccupy()
    {
        return !occupied;
    }

    public bool CouldQueue()
    {
        return queueCount < queueLocations.Count;
    }

    public bool IsOnly(CustomerGroup customerGroup)
    {
        return queueCount == 1 && queueCustomerGroups[0] == customerGroup;
    }
}
