using Invector.Utils;
using Invector.vCharacterController.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class vSpawnRangeAttack : MonoBehaviour
{
    public vControlAI controlAI;

    public GameObject rangeEffectPrefab;
    [vHelpBox("If true, the rangeEffectPrefab need a component vTargetLookAt")]
    public bool followTarget = true;
    protected virtual void Start()
    {
        if(controlAI==null)
            controlAI = GetComponentInParent<vControlAI>();

        if(controlAI)
        {
            controlAI.OnUpdateAI.AddListener(UpdateForward);
        }
    }

    public void Spawn()
    {
        GameObject obj = Instantiate(rangeEffectPrefab, transform.position, transform.rotation);
        if(followTarget)
        {
            vTargetLookAt look = obj.GetComponent<vTargetLookAt>();
            if (look) look.target = controlAI.currentTarget;
        }
    }

    protected virtual void UpdateForward()
    {
        if (controlAI.currentTarget.transform != null && controlAI.targetInLineOfSight)
        {
            transform.forward = controlAI.currentTarget.collider.bounds.center - transform.position;
        }
        else transform.forward = controlAI.transform.forward;
    }
}
