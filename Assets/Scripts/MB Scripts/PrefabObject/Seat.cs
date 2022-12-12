using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seat : MonoBehaviour
{
    public CustomerGroup customerGroup { get; private set; }

    public List<PointOfInterest> locations;

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
        this.customerGroup = customerGroup;
    }

    public void UnseatCustomerGroup()
    {
        this.customerGroup = null;
    }
}
