using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PathFinder2), typeof(MovementController2))]
public class Customer2 : MonoBehaviour
{
    private PathFinder2 pathfinder;
    private MovementController2 movement;
    private SpriteRenderer sprite;

    private CustomerGroup customerGroup;
    private CustomerTypeSO customerType;

    private void Awake()
    {
        pathfinder = GetComponent<PathFinder2>();
        movement = GetComponent<MovementController2>();
        sprite = GetComponent<SpriteRenderer>();
    }

    public void Setup(CustomerGroup customerGroup)
    {
        this.customerGroup = customerGroup;
        customerType = customerGroup.customerType;

        sprite.sprite = customerType.sprites.GetRandom();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
