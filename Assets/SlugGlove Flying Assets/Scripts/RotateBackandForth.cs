using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateBackandForth : MonoBehaviour {

    public float speed = 2f;
    public float maxRotationx = 45f;
    public float maxRotationy = 45f;
    public float maxRotationz = 45f;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.rotation = Quaternion.Euler(maxRotationx * Mathf.Sin(Time.time * speed), maxRotationy * Mathf.Sin(Time.time * speed), maxRotationz * Mathf.Sin(Time.time * speed));
    }
}
