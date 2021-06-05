using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Actors")]
    public Transform target;
    private bool detected = true;

    [Header("Head")]
    public Transform headBone;
    public float headMaxTurnAngle, headTrackingSpeed;

    [Header("Turning")]
    public float maxAngToTarget;
    public float turnSpeed;
    public float turnAcceleration;

    [Header("Movement")]
    public float maxDistToTarget;
    public float moveSpeed;
    public float moveAcceleration;

    private float currentAngularVelocity;
    private Vector3 currentVelocity;

    [Header("Left Hand")]
    public Transform leftHand;
    public Transform leftHandTarget;
    public float leftHandMaxTurnAngle;
    public float leftHandTrackingSpeed;

    [Header("Right Hand")]
    public Transform rightHand;
    public Transform rightHandTarget;
    public float rightHandMaxTurnAngle;
    public float rightHandTrackingSpeed;


    [Header("LegSteppers")]
    public bool RunSteppers = true;
    [SerializeField] LegStepper[] oppositeLegSteppers;
    [SerializeField] LegStepper[] legSteppers;

    void Start()
    {
    }

    void Awake()
    {
        StartCoroutine(LegUpdateCoroutine());
    }

    void LateUpdate() 
    {
        if (detected)
        {
            MotionUpdate();
            HeadTrackingUpdate();
            LeftHandAim();
            RightHandAim();
        }
    }

    void HeadTrackingUpdate()
    {
        //HeadRotation

        Quaternion currentLocalRotation = headBone.localRotation;
        headBone.localRotation = Quaternion.identity;

        Vector3 targetWorldLookDir = target.position - headBone.position;
        Vector3 targetLocalLookDir = headBone.InverseTransformDirection(targetWorldLookDir);

        //HeadAngle-Constraints
        targetLocalLookDir = Vector3.RotateTowards(Vector3.forward, targetLocalLookDir, Mathf.Deg2Rad * headMaxTurnAngle, 0);

        Quaternion targetLocalRotation = Quaternion.LookRotation(targetLocalLookDir, Vector3.up);
        headBone.localRotation = Quaternion.Slerp(currentLocalRotation, targetLocalRotation, 1 - Mathf.Exp(-headTrackingSpeed * Time.deltaTime));
    }

    public void LeftHandAim()
    {
        Quaternion currentLocalRotation = leftHand.localRotation;
        leftHand.localRotation = Quaternion.identity;

        Vector3 targetWorldLookDir = leftHandTarget.position - leftHand.position;
        Vector3 targetLocalLookDir = leftHand.InverseTransformDirection(targetWorldLookDir);

        targetLocalLookDir = Vector3.RotateTowards(Vector3.forward, targetLocalLookDir, Mathf.Deg2Rad * leftHandMaxTurnAngle, 0);

        Quaternion targetLocalRotation = Quaternion.LookRotation(targetLocalLookDir, Vector3.up);
        leftHand.localRotation = Quaternion.Slerp(currentLocalRotation, targetLocalRotation, 1 - Mathf.Exp(-leftHandTrackingSpeed * Time.deltaTime));

    }

    public void RightHandAim()
    {
        Quaternion currentLocalRotation = rightHand.localRotation;
        rightHand.localRotation = Quaternion.identity;

        Vector3 targetWorldLookDir = rightHandTarget.position - rightHand.position;
        Vector3 targetLocalLookDir = rightHand.InverseTransformDirection(targetWorldLookDir);

        targetLocalLookDir = Vector3.RotateTowards(Vector3.forward, targetLocalLookDir, Mathf.Deg2Rad * rightHandMaxTurnAngle, 0);

        Quaternion targetLocalRotation = Quaternion.LookRotation(targetLocalLookDir, Vector3.up);
        rightHand.localRotation = Quaternion.Slerp(currentLocalRotation, targetLocalRotation, 1 - Mathf.Exp(-rightHandTrackingSpeed * Time.deltaTime));
        
    }

    IEnumerator LegUpdateCoroutine()
    {
        while (RunSteppers)
        {
            foreach (LegStepper lg in legSteppers)
            {
                //move legs in opposite pair
                do
                {
                    lg.TryMove();

                    //Wait a frame
                    yield return null;
                }

                while (lg.moving);
            }
            foreach (LegStepper og in oppositeLegSteppers)
            {
                do
                {
                    og.TryMove();

                    yield return null;
                }
                while (og.moving);
            }
        }
    }

    void MotionUpdate()
    {
        // Look to player
        Vector3 lookVector = target.position - transform.position;
        Vector3 towardTargetProjected = Vector3.ProjectOnPlane(lookVector, transform.up);
        float angToTarget = Vector3.SignedAngle(transform.forward, towardTargetProjected, transform.up);
        float targetAngularVelocity = 0;

        if (Mathf.Abs(angToTarget) > maxAngToTarget)
        {
            if (angToTarget > 0)
            {
                targetAngularVelocity = turnSpeed;
            }
            else
            {
                //Invert the angular speed
                targetAngularVelocity = -turnSpeed;
            }
        }

        currentAngularVelocity = Mathf.Lerp(currentAngularVelocity, targetAngularVelocity, 1 - Mathf.Exp(-turnAcceleration * Time.deltaTime));

        //Rotate the transform around the Y axis in world space
        transform.Rotate(0, Time.deltaTime * currentAngularVelocity, 0, Space.World);


        // Move to Player

        //Position interpolations
        Vector3 targetVelocity = Vector3.zero;

        //Dont move if we're facing away from the target
        if (Mathf.Abs(angToTarget) < 90)
        {
            float distToTarget = Vector3.Distance(transform.position, target.position);

            //If were too far away, approach the boi
            if (distToTarget > maxDistToTarget)
            {
                targetVelocity = moveSpeed * towardTargetProjected.normalized;
            }
        }

        currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, 1 - Mathf.Exp(-moveAcceleration * Time.deltaTime));

        //apply the velocity
        transform.position += currentVelocity * Time.deltaTime;
    }

    public void DetectPlayer()
    {
        detected = true;
    }
}
