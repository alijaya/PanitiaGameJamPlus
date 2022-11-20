using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "WaveEvent", menuName = "Objective/Wave Event", order = 0)]
public class WaveSO : ScriptableObject {
    public float time;
    public  List<Customer> customers;
    public bool isMouse = false;
}