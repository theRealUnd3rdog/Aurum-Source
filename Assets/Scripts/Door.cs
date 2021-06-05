using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public enum DoorState
    {
        locked,
        open,
        closed,
    }

    public DoorState state;

    [Header("Transforms")]
    public Transform LeftDoor;
    public Transform RightDoor;
    public Transform MaxLeftPos;
    public Transform MaxRightPos;
    private Vector3 InitialLeftDoorPos;
    private Vector3 InitialRightDoorPos;

    [Header("Actors")]
    public float doorSpeed;
    public bool update;

    [Header("Audio")]
    public AudioSource doorSource;
    public AudioClip openSound;
    public AudioClip closeSound;

    void Start()
    {
        InitialLeftDoorPos = LeftDoor.position;
        InitialRightDoorPos = RightDoor.position;
    }

    void Update()
    {
        if (update)
        {
            if (state == DoorState.open)
            {
                OpenDoor();
            }

            if (state == DoorState.closed)
            {
                CloseDoor();
            }

            if (state == DoorState.locked)
            {
                LockDoor();
            }
        }
    }

    // Events
    public void OpenDoor()
    {
        Debug.Log("Open");
        state = DoorState.open;

        LeftDoor.position = Vector3.Lerp(LeftDoor.position, MaxLeftPos.position, doorSpeed * Time.deltaTime);
        RightDoor.position = Vector3.Lerp(RightDoor.position, MaxRightPos.position, doorSpeed * Time.deltaTime);

        
    }

    public void CloseDoor()
    {
        Debug.Log("Close");
        state = DoorState.closed;

        LeftDoor.position = Vector3.Lerp(LeftDoor.position, InitialLeftDoorPos, doorSpeed * Time.deltaTime);
        RightDoor.position = Vector3.Lerp(RightDoor.position, InitialRightDoorPos, doorSpeed * Time.deltaTime);
    }

    public void LockDoor()
    {
        state = DoorState.locked;
    }

    public void UnlockDoor()
    {
        state = DoorState.open;
    }

    public void PlayOpenDoor()
    {
        doorSource.clip = openSound;
        doorSource.Play();
    }

    public void PlayCloseDoor()
    {
        doorSource.clip = closeSound;
        doorSource.Play();
    }
}
