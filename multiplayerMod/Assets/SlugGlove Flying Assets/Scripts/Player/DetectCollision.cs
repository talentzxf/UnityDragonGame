using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectCollision : MonoBehaviour
{
   // public Transform MovementMesh; //what we are checking the ground based on

    public float bottomOffset;
    public float frontOffset;
    public float collisionRadius;
    public LayerMask GroundLayer;
    public float WallDistance;

    //check if there is a floor to stand on, or land on
    public bool CheckGround()
    {
        Vector3 Pos = transform.position + (-transform.up * bottomOffset);
        Collider[] hitColliders = Physics.OverlapSphere(Pos, collisionRadius, GroundLayer);
        if (hitColliders.Length > 0)
        {
            //we are on the ground
            return true;
        }

        return false;
    }

    //check if there is a wall to crash into
    public bool CheckWall()
    {
        Vector3 Pos2 = transform.position + (transform.forward * frontOffset);
        Collider[] hitColliders = Physics.OverlapSphere(Pos2, collisionRadius, GroundLayer);

        if (hitColliders.Length > 0)
        {
            return true;
        }

        return false;
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Vector3 Pos = transform.position + (-transform.up * bottomOffset);
        Gizmos.DrawSphere(Pos, collisionRadius);
        Gizmos.color = Color.red;
        Vector3 Pos2 = transform.position + (transform.forward * frontOffset);
        Gizmos.DrawSphere(Pos2, collisionRadius);
    }
}
