using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityAtoms.BaseAtoms;
using RS.Typing.Core;

public class RestaurantManager : MonoBehaviour
{
    public Transform door;
    public CounterTable counterTable;
    public List<Table> tables;

    public List<Customer> customerPrefabs;

    private List<Customer> currentCustomers;

    private List<PointOfInterest> allPOI;

    public WaveManager waveManager;

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

    private void Start()
    {
        GlobalRef.I.totalSales.Value = 0;
        GlobalRef.I.totalCustomerServed.Value = 0;
        GlobalRef.I.PlayBGM_Gameplay();
        waveManager.StartWave();
    }

    public void Spawn(IEnumerable<ItemSO> order = null)
    {
        var seat = GetAvailableSeat();
        if (seat)
        {
            var customer = Instantiate(customerPrefabs.GetRandom(), door.position, Quaternion.identity);
            customer.Setup(door, seat);
            if (order != null) customer.SetOrder(order); 
            
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
