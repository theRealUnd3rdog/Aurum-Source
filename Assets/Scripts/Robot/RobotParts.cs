using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DitzelGames.FastIK;
using UnityEngine.Animations;

public class RobotParts : MonoBehaviour
{
    private RobotTarget PartController;
    private Transform player;

    private Rigidbody rb;
    private RobotParts[] fragileParts;
    private RobotParts[] normalParts;
    private RobotParts[] bodies;

    private static HingeJoint joint;
    private Rigidbody playerRb;
    int hitIncrementer = 0;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        PartController = transform.root.GetChild(0).GetComponent<RobotTarget>();
        player = transform.root.GetChild(0).GetComponent<RobotController>().target;
    }

    public void PartChecks(float damage)
    {
        fragileParts = PartController.fragileParts;
        normalParts = PartController.normalParts;
        bodies = PartController.bodies;

        foreach (RobotParts fg in fragileParts)
        {
            if (this == fg)
            {
                Vector3 direction = fg.transform.position - player.position;

                rb.AddForce(direction.normalized * 35f, ForceMode.Impulse);

                joint = fg.GetComponent<HingeJoint>();

                Debug.Log("FragilePart hit");
                PartController.TakeDamage(damage * 2);

                hitIncrementer++;

                if (hitIncrementer == 2)
                {
                    PartController.die = true;
                    joint.GetComponentInParent<Transform>().SetParent(null);
                    joint.connectedBody = null;
                    rb.constraints = RigidbodyConstraints.None;
                    rb.isKinematic = false;
                    Destroy(joint);

                    
                    hitIncrementer = 0;
                }
            }
        }

        foreach (RobotParts np in normalParts)
        {
            if (this == np)
            {
                Vector3 direction = np.transform.position - player.position;

                rb.AddForce(direction.normalized * 35f, ForceMode.Impulse);

                Debug.Log("NormalPart hit");
                PartController.TakeDamage(damage);

                hitIncrementer++;

                Debug.Log(hitIncrementer);

                if (hitIncrementer == 3)
                {
                    joint = np.GetComponent<HingeJoint>();
                    
                    if (joint == null) return;

                    //RipOff parts
                    joint.GetComponentInParent<Transform>().SetParent(null);
                    joint.connectedBody = null;
                    rb.constraints = RigidbodyConstraints.None;
                    rb.isKinematic = false;
                    hitIncrementer = 0;

                    //Accessing children
                    Transform childJoint = joint.transform.GetChild(0);

                    Destroy(joint);

                    if (childJoint == null)
                        return;

                    childJoint.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                    childJoint.GetComponent<Rigidbody>().isKinematic = false;

                    // Disable Laser and turn it off
                    LookAtConstraint aiming = childJoint.GetComponent<LookAtConstraint>();

                    if (aiming == null)
                    {
                        // Turn off IK
                        Transform IKjoint = childJoint.transform.GetChild(0);

                        if (IKjoint == null)
                            return;

                        FastIKFabric IK = IKjoint.GetComponent<FastIKFabric>();

                        if (IK == null) return;

                        Debug.Log(IK.transform.name);

                        IK.enabled = false;
                        IK.Target = null;
                        
                        return;
                    } 

                    PartController.armAim.enabled = false;
                    PartController.TurnOffLaser();

                    
                    
                }

            }
        }

        foreach (RobotParts bd in bodies)
        {
            if (this == bd)
            {
                Debug.Log("MainBody hit");
                PartController.TakeDamage(damage);
            }
        }
    }
}
