using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;

public class SharedCustomerGroup : SharedVariable<CustomerGroup>
{
    public static implicit operator SharedCustomerGroup(CustomerGroup value) { return new SharedCustomerGroup { Value = value }; }
}
