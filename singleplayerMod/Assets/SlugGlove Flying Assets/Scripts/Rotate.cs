using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float X;
    public float Y;
    public float Z;

    public float Spd;

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 Amt = new Vector3(X, Y, Z) * Spd;
        transform.Rotate(Amt);
    }
}
