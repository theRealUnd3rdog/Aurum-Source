using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class proceduralRecoil : MonoBehaviour
{
    [Header("Reference Points")]
    public Transform recoilPosition;
    public Transform rotationPoint;

    [Header("Speed Settings")]
    public float positionalRecoilSpeed = 8f;
    public float rotationalRecoilSpeed = 8f;

    public float positionalReturnSpeed = 18f;
    public float rotationalReturnSpeed = 36f;

    [Header("Amount Settings")]
    public Vector3 recoilRotation = new Vector3(10, 5, 7);
    public Vector3 recoilKickBack = new Vector3(0.015f, 0f, -0.2f);

    public Vector3 recoilRotationAim = new Vector3(10, 4, 6);
    public Vector3 recoilKickBackAim = new Vector3(0.015f, 0f, -0.2f);

    Vector3 rotationalRecoil;
    Vector3 positionalRecoil;
    Vector3 Rot;

    [Header("State")]
    public bool aiming;

    // Start is called before the first frame update
    void LateUpdate()
    {
        rotationalRecoil = Vector3.Lerp(rotationalRecoil, Vector3.zero, rotationalReturnSpeed * Time.deltaTime);
        positionalRecoil = Vector3.Lerp(positionalRecoil, Vector3.zero, positionalRecoilSpeed * Time.deltaTime);

        recoilPosition.localPosition = Vector3.Slerp(recoilPosition.localPosition, positionalRecoil, positionalRecoilSpeed * Time.deltaTime);
        Rot = Vector3.Slerp(Rot, rotationalRecoil, rotationalRecoilSpeed * Time.deltaTime);
        rotationPoint.localRotation = Quaternion.Euler(Rot);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire2"))
        {
            aiming = true;
        }
        else 
        {
            aiming = false;
        }
    }

    public void Recoil() 
    {
        if (aiming)
        {
            rotationalRecoil += new Vector3(-recoilRotationAim.x, Random.Range(-recoilRotationAim.y, recoilRotationAim.y), Random.Range(-recoilRotationAim.z, recoilRotationAim.z));
            positionalRecoil += new Vector3(Random.Range(-recoilKickBackAim.x, recoilKickBackAim.x), Random.Range(-recoilKickBackAim.y, recoilKickBackAim.y), recoilKickBackAim.z);
        }
        else 
        {
            rotationalRecoil += new Vector3(-recoilRotation.x, Random.Range(-recoilRotation.y, recoilRotation.y), Random.Range(-recoilRotation.z, recoilRotation.z));
            positionalRecoil += new Vector3(Random.Range(-recoilKickBackAim.x, recoilKickBack.x), Random.Range(-recoilKickBack.y, recoilKickBack.y), recoilKickBack.z);
        }
    }
}
