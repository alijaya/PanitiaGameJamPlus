using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;

public class SharedCustomerTypeSO : SharedVariable<CustomerTypeSO>
{
    public static implicit operator SharedCustomerTypeSO(CustomerTypeSO value) { return new SharedCustomerTypeSO { Value = value }; }
}
