using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;

public class SharedSeat : SharedVariable<Seat>
{
    public static implicit operator SharedSeat(Seat value) { return new SharedSeat { Value = value }; }
}
