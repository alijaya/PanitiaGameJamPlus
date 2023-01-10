using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Game")]
public class GetAvailableSeat : Action
{
    public SharedSeat seat;
    public SharedTransform target;

    public override TaskStatus OnUpdate()
    {
        seat.Value = SeatManager.I.GetAvailableSeat();
        target.Value = seat.Value.locations[0].transform;
        return TaskStatus.Success;
    }
}
