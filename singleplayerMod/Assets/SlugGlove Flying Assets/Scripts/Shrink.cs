using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shrink : MonoBehaviour
{
    public float Speed;
    private float ShrinkAmt;

    // Start is called before the first frame update
    public void Setself()
    {
        ShrinkAmt = 0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ShrinkAmt += Time.deltaTime * Speed;

        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, ShrinkAmt);

        if (ShrinkAmt > 1.1)
            GetComponent<Shrink>().enabled = false;
    }
}
