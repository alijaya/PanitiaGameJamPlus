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
    public bool isChair = false;
    public float seatHeight = .5f;

    public GameObject occupyObject;
}
