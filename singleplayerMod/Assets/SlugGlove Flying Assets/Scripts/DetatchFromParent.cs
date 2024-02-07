using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetatchFromParent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.parent = null;
        Destroy(GetComponent<DetatchFromParent>());
    }
}
