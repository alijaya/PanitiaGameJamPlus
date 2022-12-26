using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonDoor : SingletonMB<SingletonDoor>
{
    public override bool IsPersistent { get; } = false;
}
