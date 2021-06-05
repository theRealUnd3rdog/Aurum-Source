using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject impactHole;
    public LayerMask mask;

    /*
    void OnCollisionEnter(Collision collisionInfo)
    {
        if (collisionInfo.gameObject.CompareTag("Robot"))
        {
            RobotParts enemyParts = collisionInfo.transform.GetComponent<RobotParts>();

            if (enemyParts != null) 
            {
                enemyParts.PartChecks(Weapon.weaponInstance.damage);
            }
        }
        
        if (collisionInfo.rigidbody != null)
        {
            for (int i = 0; i < collisionInfo.contacts.Length; i++)
            {
                collisionInfo.rigidbody.AddForce(-collisionInfo.contacts[i].normal * Weapon.weaponInstance.impactForce);

                GameObject impact = Instantiate(impactHole, collisionInfo.contacts[i].point, Quaternion.LookRotation(collisionInfo.contacts[i].normal));
                Destroy(impact, 3.5f);
            }
        }

        Destroy(this, 0.2f);
        
    }*/

    void Update()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 2f)) 
        {
            
            Debug.Log(hit.transform.name);

            RobotParts enemyParts = hit.transform.GetComponent<RobotParts>();

            if (hit.rigidbody != null)
                hit.rigidbody.AddForce(-hit.normal * Weapon.weaponInstance.impactForce);
            
            
            if (enemyParts != null) 
            {
                enemyParts.PartChecks(Weapon.weaponInstance.damage);
            }
        }
    } 

    void OnTriggerEnter(Collider collider) 
    {
        Destroy(this.transform.parent.gameObject);
    }
}
