using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Seat : MonoBehaviour
{
    public CustomerGroup customerGroup { get; private set; }
    public CustomerGroup waitCustomerGroup { get; private set; }

    public PointOfInterest serveLocation;
    public List<PointOfInterest> locations;
    public List<PointOfInterest> waitLocations;

    public UnityEvent<CustomerGroup> OnSeatOccupied = new ();
    public UnityEvent<CustomerGroup> OnSeatUnoccupied = new ();

    public bool occupied
    {
        get
        {
            return customerGroup != null;
        }
    }

    public int count
    {
        get
        {
            return locations.Count;
        }
    }

    public bool waitOccupied
    {
        get
        {
            return waitCustomerGroup != null;
        }
    }

    public int waitCount
    {
        get
        {
            return waitLocations.Count;
        }
    }

    private void OnEnable()
    {
        SeatManager.I.RegisterSeat(this);
    }

    private void OnDisable()
    {
        SeatManager.I.UnregisterSeat(this);
    }

    public void SeatCustomerGroup(CustomerGroup customerGroup)
    {
        if (this.customerGroup == null)
        {
            this.customerGroup = customerGroup;
            OnSeatOccupied.Invoke(customerGroup);
        }
    }

    public void UnseatCustomerGroup(CustomerGroup customerGroup)
    {
        if (this.customerGroup == customerGroup)
        {
            var temp = this.customerGroup;
            this.customerGroup = null;
            OnSeatUnoccupied.Invoke(temp);
        }
    }

    public void WaitCustomerGroup(CustomerGroup customerGroup)
    {
        if (this.waitCustomerGroup == null)
        {
            this.waitCustomerGroup = customerGroup;
        }
    }

    public void UnwaitCustomerGroup(CustomerGroup customerGroup)
    {
        if (this.waitCustomerGroup == customerGroup)
        {
            var temp = this.waitCustomerGroup;
            this.waitCustomerGroup = null;
        }
    }

    public bool CouldSeat(int customerCount)
    {
        return !occupied && !waitOccupied && count >= customerCount;
    }

    public bool CouldSeat(CustomerGroup customerGroup)
    {
        return CouldSeat(customerGroup.count);
    }

    public bool CouldWait(int customerCount)
    {
        return !waitOccupied && waitLocations.Count >= customerCount;
    }

    public bool CouldWait(CustomerGroup customerGroup)
    {
        return CouldWait(customerGroup.count);
    }
}
