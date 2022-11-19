using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityAtoms.BaseAtoms;

public class RestaurantManager : MonoBehaviour
{
    public Transform door;
    public CounterTable counterTable;
    public List<Table> tables;

    public List<Customer> customerPrefabs;

    private List<Customer> currentCustomers;

    private List<PointOfInterest> allPOI;

    private void Awake()
    {
        currentCustomers = new List<Customer>();
        allPOI = new List<PointOfInterest>();
        allPOI.AddRange(counterTable.counterLocations);
        foreach (var table in tables)
        {
            allPOI.Add(table.left);
            allPOI.Add(table.right);
        }
        GlobalRef.I.CleanUpWords();
    }

    public void Spawn()
    {
        var seat = GetAvailableSeat();
        if (seat)
        {
            var customer = Instantiate(customerPrefabs.GetRandom(), door.position, Quaternion.identity);
            customer.Setup(door, seat);
            currentCustomers.Add(customer);
        }
    }

    public void RandomLeave()
    {
        var arrivedCustomers = currentCustomers.FindAll(c => c.IsArrived);
        if (arrivedCustomers.Count > 0)
        {
            var customer = arrivedCustomers.GetRandom();
            currentCustomers.Remove(customer);
            customer.Leave();
        }
    }

    public PointOfInterest GetAvailableSeat()
    {
        var availablePOI = allPOI.FindAll(poi => !poi.occupyObject);
        if (availablePOI.Count == 0) return null;
        return availablePOI.GetRandom();
    }

    private void Update()
    {

    }
}
