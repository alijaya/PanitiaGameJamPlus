using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ObjectPathFinder), typeof(MovementController))]
public class Customer2 : MonoBehaviour
{
    private ObjectPathFinder pathfinder;
    private MovementController movement;
    private SpriteRenderer sprite;

    private CustomerGroup customerGroup;
    private CustomerTypeSO customerType;

    private void Awake()
    {
        pathfinder = GetComponent<ObjectPathFinder>();
        movement = GetComponent<MovementController>();
        sprite = GetComponent<SpriteRenderer>();
        movement.sprite = sprite;
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
