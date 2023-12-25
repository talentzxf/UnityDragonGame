using System;
using System.Collections;
using System.Collections.Generic;
using DragonGameNetworkProject.DragonMovements;
using UnityEngine;

public class DragonGroundDetector : MonoBehaviour
{
    private DragonLandMovement landMovement;
    
    private void Awake()
    {
        landMovement = GetComponentInParent<DragonLandMovement>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Terrain"))
        {
            landMovement.onGround = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Terrain"))
        {
            landMovement.onGround = false;
        }
    }
}
