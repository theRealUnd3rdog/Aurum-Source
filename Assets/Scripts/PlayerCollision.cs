using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public float FloorCheckRadius;
    public float bottomOffset;
    public float WallCheckRadius;
    public float frontOffset; 
    public float RoofCheckRadius;
    public float upOffset;

    public float LedgeGrabForwardPos;
    public float LedgeGrabUpwardsPos;
    public float LedgeGrabDistance;

    public LayerMask FloorLayers; 
    public LayerMask WallLayers;  
    public LayerMask RoofLayers; 
    public LayerMask LedgeGrabLayers; 

    public bool CheckFloor(Vector3 Direction)
    {
        Vector3 Pos = transform.position + (Direction * bottomOffset);
        Collider[] hitColliders = Physics.OverlapSphere(Pos, FloorCheckRadius, FloorLayers);
        if (hitColliders.Length > 0)
        {
            //we are on the ground
            return true;
        }

        return false;
    }
    //check if there is a wall in the direction we are pressing
    public bool CheckWall(Vector3 Direction)
    {
        Vector3 Pos = transform.position + (Direction * frontOffset);
        Collider[] hitColliders = Physics.OverlapSphere(Pos, WallCheckRadius, WallLayers);
        if (hitColliders.Length > 0)
        {
            //we are on the ground
            return true;
        }

        return false;
    }


    // Get normal of wall to perform wall jump direction
    public Vector3 GetWallNormal()
    {
        Vector3 Pos = transform.position + (transform.forward * frontOffset);
        Collider[] hitColliders = Physics.OverlapSphere(Pos, WallCheckRadius, WallLayers);

        Vector3[] wallPoint = new Vector3[hitColliders.Length];
        Vector3[] dir = new Vector3[hitColliders.Length];
        RaycastHit[] hit = new RaycastHit[hitColliders.Length];

        for (int i = 0; i < hitColliders.Length; i++)
        {
            wallPoint[i] = hitColliders[i].ClosestPointOnBounds(transform.position);
            dir[i] = (wallPoint[i] - transform.position).normalized;
            
            if (Physics.Raycast(transform.position, dir[i], out hit[i], WallLayers))
            {
                return hit[i].normal;
            }
        }
        

        return Vector3.zero;
    }

    //check there is nothing above our head so we can stand up
    public bool CheckRoof(Vector3 Direction)
    {
        Vector3 Pos = transform.position + (Direction * upOffset);
        Collider[] hitColliders = Physics.OverlapSphere(Pos, WallCheckRadius, RoofLayers);
        if (hitColliders.Length > 0)
        {
            //we are on the ground
            return true;
        }

        return false;
    }

    public Vector3 CheckLedges()
    {
        Vector3 RayPos = transform.position + (transform.forward * LedgeGrabForwardPos) + (transform.up * LedgeGrabUpwardsPos);

        RaycastHit hit;
        if (Physics.Raycast(RayPos, -transform.up, out hit, LedgeGrabDistance, LedgeGrabLayers))
            return hit.point;


        return Vector3.zero;
    }

    void OnDrawGizmosSelected()
    {
        //floor check
        Gizmos.color = Color.yellow;
        Vector3 Pos = transform.position + (-transform.up * bottomOffset);
        Gizmos.DrawSphere(Pos, FloorCheckRadius);
        //wall check
        Gizmos.color = Color.red;
        Vector3 Pos2 = transform.position + (transform.forward * frontOffset);
        Gizmos.DrawSphere(Pos2, WallCheckRadius);
        //roof check
        Gizmos.color = Color.green;
        Vector3 Pos3 = transform.position + (transform.up * upOffset);
        Gizmos.DrawSphere(Pos3, RoofCheckRadius);
        //Ledge check
        Gizmos.color = Color.black;
        Vector3 Pos4 = transform.position + (transform.forward * LedgeGrabForwardPos) + (transform.up * LedgeGrabUpwardsPos);
        Gizmos.DrawLine(Pos4, Pos4 + (-transform.up * LedgeGrabDistance));
    }
}
