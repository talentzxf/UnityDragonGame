using System;
using System.Collections;
using UnityEngine;
using Object = System.Object;

// Since when player is sit on dragon, IK should always work.
public abstract class OnDragonLocomotionProcessor : AnimatedCharactorLocomotionProcessor
{
    private String leftFootName = "LeftFoot";
    private String rightFootName = "RightFoot";

    private Transform _leftFootIK;
    private Transform _rightFootIK;

    protected GameObject _dragonGO;
    protected Transform _dragonTransform;

    public override void OnActive(params object[] parameters)
    {
        base.OnActive(parameters);

        GameObject dragonGO = parameters[0] as GameObject;

        _dragonGO = dragonGO;
        _dragonTransform = dragonGO.transform;
    }

    public override void OnAnimatorIK(int layerIndex)
    {
        float climbUpProgress = _animator.GetFloat("ClimbUpProgress");
        if (climbUpProgress > 0.5)
        {
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, climbUpProgress);
            _animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, climbUpProgress);

            _animator.SetIKPosition(AvatarIKGoal.LeftFoot, GetLeftFootIK().position);
            _animator.SetIKPosition(AvatarIKGoal.RightFoot, GetRightFootIK().position);
        }
    }

    Transform GetLeftFootIK()
    {
        if (_leftFootIK == null)
        {
            _leftFootIK = Utility.RecursiveFind(_dragonTransform, leftFootName);
        }

        return _leftFootIK;
    }

    Transform GetRightFootIK()
    {
        if (_rightFootIK == null)
        {
            _rightFootIK = Utility.RecursiveFind(_dragonTransform, rightFootName);
        }

        return _rightFootIK;
    }
}

public class ClimbDragonProcessor : OnDragonLocomotionProcessor
{
    private CharacterController _character;

    private String sitPoint = "SitPoint";

    private String dragonNeckName = "Bone.008";


    private int _climb = Animator.StringToHash("climb");

    private Vector3 climbStartForward;

    public override void Setup(CharacterLocomotion locomotion)
    {
        base.Setup(locomotion);
        _character = _go.GetComponent<CharacterController>();
    }

    IEnumerator FixPlayerPosition(float durationSeconds)
    {
        Vector3 startPosition = _transform.position;
        Vector3 endPosition = Utility.RecursiveFind(_dragonTransform, sitPoint).position;

        float percentage = 0.0f;
        while (percentage < durationSeconds)
        {
            _transform.position = Vector3.Slerp(startPosition, endPosition, percentage);
            percentage += Time.deltaTime;
            yield return null;
        }
    }

    public override void OnActive(params Object[] parameters)
    {
        base.OnActive(parameters);

        Vector3 startPosition = (Vector3) parameters[1];
        Vector3 forwardDir = _dragonTransform.right;

        // Align rotation
        _transform.position = startPosition;
        _transform.forward = forwardDir;

        climbStartForward = forwardDir;

        _character.enabled = false;

        _animator.SetTrigger(_climb);

        Camera.main.GetComponent<CameraController>().LerpToDistance(2.0f);
    }

    public override void Update()
    {
        float climbUpProgress = _animator.GetFloat("ClimbUpProgress");
        Vector3 currentForward = Vector3.Lerp(climbStartForward, _dragonTransform.forward, climbUpProgress);
        _transform.forward = currentForward;

        if (climbUpProgress > 0.99f)
        {
            _loco.StartCoroutine(FixPlayerPosition(0.5f));
            Transform bone08 = Utility.RecursiveFind(_dragonTransform, dragonNeckName);
            _transform.SetParent(bone08);
            _dragonGO.GetComponent<CharacterLocomotion>().SetProcessor<DragonMounted>();

            _loco.SetProcessor<OnDragonProcessor>(_dragonGO);
        }
    }
}