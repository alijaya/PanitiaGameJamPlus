using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;

public class SharedCustomer : SharedVariable<Customer2>
{
    public static implicit operator SharedCustomer(Customer2 value) { return new SharedCustomer { Value = value }; }
}
