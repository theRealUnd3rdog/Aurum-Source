using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drill : MonoBehaviour
{
    public Transform drill;
    public float drillSpeed;
    public float drillDamage;
    private bool isCollision = false;
    private PlayerHealth health;

    void FixedUpdate()
    {
        drill.Rotate(0, 0, drillSpeed);

        if (isCollision) health.health -= drillDamage;
    }

    void OnCollisionEnter(Collision collisionInfo)
    {
        if (collisionInfo.gameObject.CompareTag("Player"))
        {
            health = collisionInfo.transform.GetComponent<PlayerHealth>();
            isCollision = true;
        }
    }

    void OnCollisionExit(Collision collisionInfo)
    {
        if (collisionInfo.gameObject.CompareTag("Player"))
        {
            health = collisionInfo.transform.GetComponent<PlayerHealth>();
            isCollision = false;
        }
    }
}
