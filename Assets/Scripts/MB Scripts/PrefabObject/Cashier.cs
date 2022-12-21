using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Cashier : MonoBehaviour
{
    public CustomerGroup customerGroup { get; private set; }
    public HashSet<CustomerGroup> queueCustomerGroups { get; private set; } = new();

    public PointOfInterest location;
    public List<PointOfInterest> queueLocations;

    public UnityEvent<CustomerGroup> OnCashierOccupied = new();
    public UnityEvent<CustomerGroup> OnCashierUnoccupied = new();

    public bool occupied
    {
        get
        {
            return customerGroup != null;
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

    public void OccupyCustomerGroup(CustomerGroup customerGroup)
    {
        if (this.customerGroup == null)
        {
            this.customerGroup = customerGroup;
            OnCashierOccupied.Invoke(customerGroup);
        }
    }

    public void UnoccupyCustomerGroup(CustomerGroup customerGroup)
    {
        if (this.customerGroup == customerGroup)
        {
            var temp = this.customerGroup;
            this.customerGroup = null;
            OnCashierUnoccupied.Invoke(temp);
        }
    }

    public void QueueCustomerGroup(CustomerGroup customerGroup)
    {
        this.queueCustomerGroups.Add(customerGroup);
    }

    public void UnqueueCustomerGroup(CustomerGroup customerGroup)
    {
        this.queueCustomerGroups.Remove(customerGroup);
    }

    public bool CouldOccupy()
    {
        return !occupied && queueCount == 0;
    }
}
