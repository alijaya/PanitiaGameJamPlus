using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerGroup : MonoBehaviour
{
    public CustomerTypeSO customerType;
    public int count = 1;

    public List<Customer> customers = new List<Customer>();

    // Start is called before the first frame update
    void Start()
    {
        if (!customerType) return;
        for (var i = 0; i < count; i++)
        {
            var customerGO = new GameObject(customerType.name);
            var customer = customerGO.AddComponent<Customer>();
            customers.Add(customer);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
