using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fan : MonoBehaviour
{
    public Transform fan;
    public float fanSpeed;
    void FixedUpdate()
    {
        fan.Rotate(0, 0, fanSpeed);
    }
}
