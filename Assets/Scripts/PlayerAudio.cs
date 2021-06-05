using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    public PlayerMovement movement;
    public AudioSource[] sources;

    public void PlayLeftFoot(float verticalSpeed)
    {
        
        float actualSpeed = movement.Vel.z;

        if (getMovementState(verticalSpeed) == getMovementState(actualSpeed))
        {
            Debug.Log("left foot");
            sources[0].pitch = Random.Range(0.85f, 1.2f);
            sources[0].Play();
        }
    }

    public void PlayRightFoot(float verticalSpeed)
    {
        
        float actualSpeed = movement.Vel.z;

        if (getMovementState(verticalSpeed) == getMovementState(actualSpeed))
        {
            Debug.Log("right foot");
            sources[1].pitch = Random.Range(0.85f, 1.2f);
            sources[1].Play();
        }
    }

    public void PlayWeaponRFoot()
    {
        sources[1].pitch = Random.Range(0.89f, 1.1f);
        sources[1].Play();
    }

    public void PlayWeaponLFoot()
    {
        sources[0].pitch = Random.Range(0.9f, 1.1f);
        sources[0].Play();
    }

    private int getMovementState(float speed)
    {
        if (speed < -2) return 0;
        if (speed < 10f) return 1;
        if (speed < 13.5f) return 2;

        return 3;
    }
}
