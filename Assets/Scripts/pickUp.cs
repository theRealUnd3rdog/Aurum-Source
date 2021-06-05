using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickUp : MonoBehaviour
{
    [HideInInspector]
    public static List<pickUp> Attractors;

    [HideInInspector]
    public Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable() 
    {
        if (Attractors == null) 
            Attractors = new List<pickUp>();

        Attractors.Add(this);
    }

    void OnDisable() 
    {
        Attractors.Remove(this);
    }
}
