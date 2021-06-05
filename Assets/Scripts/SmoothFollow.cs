using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollow : MonoBehaviour
{

    public float smoothSpeed = 15f;

    private Quaternion localRot;
    private Quaternion curRot;

    void Start()
    {
        localRot = transform.localRotation;
        curRot = transform.rotation;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Quaternion targetRot = transform.parent.rotation;

        curRot = Quaternion.Lerp(curRot, targetRot, smoothSpeed * Time.deltaTime);
        transform.rotation = curRot;
    }
}
