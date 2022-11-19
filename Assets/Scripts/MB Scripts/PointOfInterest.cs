using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointOfInterest : MonoBehaviour
{
    public enum StopDirection
    {
        None,
        Left,
        Right,
    }

    public StopDirection stopDirection;

    public GameObject occupyObject;
}
