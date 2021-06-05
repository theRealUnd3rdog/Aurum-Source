using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    [Header("Actors")]
    public Transform target;
    public bool running = true;

    [Header("Head")]
    [SerializeField] Transform headBone;
    [SerializeField] float headMaxTurnAngle, headTrackingSpeed;

    [Header("LegSteppers")]
    [SerializeField] LegStepper[] oppositeLegSteppers;
    [SerializeField] LegStepper[] legSteppers;

    [Header("Turn floats")]
    [SerializeField] float turnSpeed;
    [SerializeField] float turnAcceleration;

    [Header("Move floats")]
    [SerializeField] float moveSpeed;
    [SerializeField] bool moveBack;
    [SerializeField] float moveAcceleration;

    [Header("Distances")]
    [SerializeField] float minDistToTarget;
    [SerializeField] float maxDistToTarget;
    [SerializeField] float maxAngToTarget;

    [Header("Detection")]
    [SerializeField] float detectionRange;
    public bool playerDetected = false;
    public LayerMask detectionMask;

    Vector3 currentVelocity;
    float currentAngularVelocity;

    void LateUpdate()
    {
        DetectPlayer();

        if (playerDetected)
        {
            RootMotionUpdate();
            HeadTrackingUpdate();
        }
    }

    void Awake()
    {
        StartCoroutine(LegUpdateCoroutine());
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

    IEnumerator LegUpdateCoroutine()
    {
        //Keep running continously
        while (running)
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

    void RootMotionUpdate()
    {
        //direction toward our target
        Vector3 towardTarget = target.position - transform.position;
        //Vector toward target on XY plane
        Vector3 towardTargetProjected = Vector3.ProjectOnPlane(towardTarget, transform.up);

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

        //smoothing
        currentAngularVelocity = Mathf.Lerp(currentAngularVelocity, targetAngularVelocity, 1 - Mathf.Exp(-turnAcceleration * Time.deltaTime));

        //Rotate the transform around the Y axis in world space
        transform.Rotate(0, Time.deltaTime * currentAngularVelocity, 0, Space.World);


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
            //if we're too close, reverse the direction and move away
            else if (distToTarget < minDistToTarget && moveBack)
            {
                targetVelocity = moveSpeed * -towardTargetProjected.normalized;
            }
        }

        currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, 1 - Mathf.Exp(-moveAcceleration * Time.deltaTime));

        //apply the velocity
        transform.position += currentVelocity * Time.deltaTime;
    }

    public bool DetectPlayer()
    {
        Debug.DrawLine(transform.position, target.position, Color.red);

        Vector3 direction = target.position - transform.position;
        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, detectionRange, detectionMask))
        {
            if (hit.transform.CompareTag("Player")) playerDetected = true;
            else Invoke("DetectionDelay", 3f);
        }
        else
        {
            playerDetected = false;
        }

        return playerDetected;
    }

    private void DetectionDelay()
    {
        playerDetected = false;
    }
}
