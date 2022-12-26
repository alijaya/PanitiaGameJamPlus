using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawnCustomer : AbstractWaveDurationTrigger
{
    public enum EatType
    {
        Mix,
        DineIn,
        TakeOut,
    };

    public EatType eatType = EatType.Mix;

    public List<int> customerCounts;

    public List<CustomerTypeSO> customerTypes;

    public override void Trigger()
    {
        var customerType = customerTypes.GetRandom();
        var customerCount = customerCounts.GetRandom();
        var dineIn = new[] { Random.value < 0.5, true, false }[(int) eatType];

        CustomerGroup.Spawn(customerType, dineIn, customerCount, SingletonDoor.I.transform);
    }
}
