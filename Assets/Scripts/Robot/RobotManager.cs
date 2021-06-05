using DitzelGames.FastIK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotManager : MonoBehaviour
{
    [SerializeField] Rigidbody bodyRb;

    [Header("Arrays")]
    [SerializeField] Rigidbody[] jointRb;
    [SerializeField] LegStepper[] legSteppers;
    private HingeJoint[] joints;

    [SerializeField] FastIKFabric[] IK;

    [Header("Manager")]
    public bool ragdoll = false;
    public bool inverseKinematics = false;
    public bool steppers = false;

    // Start is called before the first frame update
    void Awake()
    {
        joints = new HingeJoint[jointRb.Length];
    }

    void Start()
    {
        GetJoints();
    }

    // Update is called once per frame
    void Update()
    {
        if (inverseKinematics)
            return; //TurnOnIK();
        else
            TurnOffIK();

        if (ragdoll)
            TurnOnRagdoll();
        else
            //TurnOffRagdoll();

        if (steppers)
            EnableLegSteppers();
        else
            DisableLegSteppers();
    }

    private void GetJoints()
    {
        for (int i = 0; i < jointRb.Length; i++)
        {
            joints[i] = jointRb[i].GetComponent<HingeJoint>();
        }
    }

    private void TurnOffIK()
    {
        foreach (FastIKFabric ik in IK)
        {
            ik.enabled = false;
        }
    }

    private void TurnOnIK()
    {
        foreach (FastIKFabric ik in IK)
        {
            ik.enabled = true;
        }
    }

    private void TurnOnRagdoll() 
    {
        foreach (Rigidbody r in jointRb) 
        {
            r.constraints = RigidbodyConstraints.None;
            r.isKinematic = false;
        }
        bodyRb.isKinematic = false;
        bodyRb.constraints = RigidbodyConstraints.None;
    }

    private void TurnOffRagdoll()
    {
        foreach (Rigidbody r in jointRb)
        {
            r.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        }
    }

    private void DisableLegSteppers() 
    {
        foreach (LegStepper l in legSteppers) 
        {
            l.enabled = false;
        }
    }

    private void EnableLegSteppers()
    {
        foreach (LegStepper l in legSteppers) 
        {
            l.enabled = true;
        }
    }

    public void RemoveHomeRigid()
    {
        foreach(LegStepper l in legSteppers)
        {
            Destroy(l.GetComponent<Rigidbody>());
        }
    }
}
