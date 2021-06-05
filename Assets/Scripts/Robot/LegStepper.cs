using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class LegStepper : MonoBehaviour
{
    [Header("Actors")]
    public Transform homeTarget;
    [SerializeField] float groundedDistance;
    [SerializeField] float stepAtDistance;
    [SerializeField] float moveDuration;
    [SerializeField] float stepOvershootFraction;

    [Header("Audio")]
    [SerializeField] AudioSource moveAudio;

    [Header("Checks")]
    public bool moving;

    public void TryMove()
    {

        if (moving) return;

        float distFromHome = Vector3.Distance(transform.position, homeTarget.position);

        if (distFromHome > stepAtDistance) 
        {
            StartCoroutine(MoveToHome());

            
        }
    }

    IEnumerator MoveToHome() 
    {
        moving = true;

        Quaternion startRot = transform.rotation;
        Vector3 startPoint = transform.position;

        Quaternion endRot = homeTarget.rotation;

        //Directional Vector
        Vector3 towardHome = (homeTarget.position - transform.position);

        //Total distance to overshoot
        float overshootDistance = stepAtDistance * stepOvershootFraction;
        Vector3 overshootVector = towardHome * overshootDistance;

        //groundRestrictions
        overshootVector = Vector3.ProjectOnPlane(overshootVector, Vector3.up);

        //Apply the overshoot
        Vector3 endPoint = homeTarget.position + overshootVector;

        //Get the centerPoint and lift off at half the step distance
        Vector3 centerPoint = (startPoint + endPoint) / 2;
        centerPoint += homeTarget.up * Vector3.Distance(startPoint, endPoint) / 2f;

        //Time since step started
        float timeElapsed = 0;

        moveAudio.pitch = Random.Range(0.9f, 1.1f);
        moveAudio.Play();

        //do while loop so the normalized time goes past 1 on the last iteration
        //placing at the end pos before ending

        do
        {
            timeElapsed += Time.deltaTime;

            float normalizedTime = timeElapsed / moveDuration;
            normalizedTime = Easing.InCubic(normalizedTime);

            //Quadratic beziur curve and interpolation
            transform.position = Vector3.Lerp(Vector3.Lerp(startPoint, centerPoint, normalizedTime),
                Vector3.Lerp(centerPoint, endPoint, normalizedTime), normalizedTime);
            transform.rotation = Quaternion.Slerp(startRot, endRot, normalizedTime);

            //Wait for one frame
            yield return null;
        }
        while (timeElapsed < moveDuration);

        //moving is done
        moving = false;
    }
}
