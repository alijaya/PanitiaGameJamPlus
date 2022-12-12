using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

[RequireComponent(typeof(PathFinder), typeof(MovementController), typeof(CustomCoordinate))]
public class Customer2 : MonoBehaviour
{
    private PathFinder pathfinder;
    private MovementController movement;
    private CustomCoordinate coordinate;

    public CustomerTypeSO customerType;

    private void Awake()
    {
        pathfinder = GetComponent<PathFinder>();
        movement = GetComponent<MovementController>();
        coordinate = GetComponent<CustomCoordinate>();
    }
}
