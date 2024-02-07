using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PrintStateAuthority : NetworkBehaviour
{
    private NetworkObject _networkObject;
    private void Awake()
    {
        _networkObject = GetComponent<NetworkObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_networkObject.StateAuthority == null)
        {
            Debug.Log("Dragon State Authority is null");
        }
        
        if (_networkObject.HasInputAuthority == null)
        {
            Debug.Log("Dragon Input Authority is null");
        }
    }
}
