using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RestaurantManager : MonoBehaviour
{
    public Transform door;
    public CounterTable counterTable;
    public List<Table> tables;

    public Costumer costumerPrefab;

    private List<PointOfInterest> allPOI;

    private void Start()
    {
        allPOI = new List<PointOfInterest>();
        allPOI.AddRange(counterTable.counterLocations);
        foreach (var table in tables)
        {
            allPOI.Add(table.left);
            allPOI.Add(table.right);
        }
    }

    public void Spawn()
    {
        var seat = GetAvailableSeat();
        if (seat)
        {
            var costumer = Instantiate(costumerPrefab, door.position, Quaternion.identity);
            costumer.Setup(seat);
        }
    }

    public PointOfInterest GetAvailableSeat()
    {
        var availablePOI = allPOI.FindAll(poi => !poi.occupyObject);
        if (availablePOI.Count == 0) return null;
        return availablePOI[Random.Range(0, availablePOI.Count)];
    }

    private void Update()
    {

    }
}
