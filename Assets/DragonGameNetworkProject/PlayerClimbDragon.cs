using System.Collections;
using Fusion;
using Fusion.Addons.FSM;
using UnityEngine;

public class PlayerClimbDragon : StateBehaviour
{
    private string sitPoint = "SitPoint";
    private string dragonNeckName = "Bone.008";
    private int _climb = Animator.StringToHash("climb");

    private CharacterController _cc;
    private GameObject dragonGO;
    private Transform dragonTransform;
    private Transform _transform;
    private NetworkMecanimAnimator _animator;

    private Vector3 climbStartForward;

    public void Setup(GameObject dragonGO, Vector3 startPosition)
    {
        dragonTransform = dragonGO.transform;
        Vector3 forwardDir = dragonTransform.right;
        _transform.position = startPosition;
        _transform.forward = forwardDir;

        climbStartForward = forwardDir;

        _cc.enabled = false;

        _animator.Animator.SetBool(_climb, true);

        Camera.main.GetComponent<FirstPersonCamera>().LerpToDistance(3.0f, 3.0f);
    }

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            GameObject outParent = GetComponentInParent<PlayerLocomotion>().gameObject;
            _cc = outParent.GetComponentInChildren<CharacterController>();
            _transform = _cc.transform;
            _animator = outParent.GetComponentInChildren<NetworkMecanimAnimator>();
        }
    }

    IEnumerator FixPlayerPosition(float durationSeconds)
    {
        Vector3 startPosition = _transform.position;
        Vector3 endPosition = Utility.RecursiveFind(dragonTransform, sitPoint).position;

        float progress = 0.0f;
        while (progress < durationSeconds)
        {
            _transform.position = Vector3.Slerp(startPosition, endPosition, progress / durationSeconds);
            progress += Time.deltaTime;
            yield return null;
        }
    }

    protected override void OnFixedUpdate()
    {
        if (!HasStateAuthority)
        {
            return;
        }

        float climbUpProgress = _animator.Animator.GetFloat("ClimbUpProgress");
        Vector3 currentForward = Vector3.Lerp(climbStartForward, dragonTransform.forward, climbUpProgress);
        _transform.forward = currentForward;

        if (climbUpProgress > 0.0)
        {
            _animator.Animator.SetBool(_climb, false);
        }

        if (climbUpProgress > 0.99f)
        {
            StartCoroutine(FixPlayerPosition(0.5f));
            Transform bone08 = Utility.RecursiveFind(dragonTransform, dragonNeckName);
            _transform.SetParent(bone08);
            // dragonGO.GetComponent<CharacterLocomotion>().SetProcessor<DragonMounted>();

            // _loco.SetProcessor<OnDragonProcessor>(_dragonGO);
        }
    }
}