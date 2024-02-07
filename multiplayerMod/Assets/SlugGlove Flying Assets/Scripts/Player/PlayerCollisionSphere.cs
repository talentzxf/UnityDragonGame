using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionSphere : MonoBehaviour
{
    [HideInInspector]
    public PlayerMovement PlayerMov;

    public void Setup(PlayerMovement Mov)
    {
        PlayerMov = Mov;
    }
}
