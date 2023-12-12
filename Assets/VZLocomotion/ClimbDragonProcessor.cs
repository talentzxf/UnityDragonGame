using System;
using System.Collections;
using UnityEngine;

namespace DefaultNamespace.VZLocomotion
{
    public class ClimbDragonProcessor : ICharacterLocomotionProcessor
    {
        private GameObject _dragonGO;
        private Transform dragonTransform;
        private CharacterController _character;
        private Animator _animator;

        private String sitPoint = "SitPoint";

        private String dragonNeckName = "Bone.008";
        private String leftFootName = "LeftFoot";
        private String rightFootName = "RightFoot";

        private int _climb = Animator.StringToHash("climb");

        private Vector3 climbStartForward;

        private Transform _leftFootIK;
        private Transform _rightFootIK;

        public override void Setup(CharacterLocomotion locomotion)
        {
            base.Setup(locomotion);
            _animator = _go.GetComponent<Animator>();
            _character = _go.GetComponent<CharacterController>();
        }

        IEnumerator FixPlayerPosition(float durationSeconds)
        {
            Vector3 startPosition = _transform.position;
            Vector3 endPosition = Utility.RecursiveFind(dragonTransform, sitPoint).position;

            float percentage = 0.0f;
            while (percentage < durationSeconds)
            {
                _transform.position = Vector3.Slerp(startPosition, endPosition, percentage);
                percentage += Time.deltaTime;
                yield return null;
            }
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
                _leftFootIK = Utility.RecursiveFind(dragonTransform, leftFootName);
            }

            return _leftFootIK;
        }

        Transform GetRightFootIK()
        {
            if (_rightFootIK == null)
            {
                _rightFootIK = Utility.RecursiveFind(dragonTransform, rightFootName);
            }

            return _rightFootIK;
        }

        public override void OnActive(params System.Object[] parameters)
        {
            Vector3 startPosition = (Vector3) parameters[0];
            GameObject dragonGO = parameters[1] as GameObject;

            _dragonGO = dragonGO;
            dragonTransform = dragonGO.transform;
            Vector3 forwardDir = dragonTransform.right;

            // Align rotation
            _transform.position = startPosition;
            _transform.forward = forwardDir;

            climbStartForward = forwardDir;

            _character.enabled = false;

            _animator.SetTrigger(_climb);
        }

        public override void Update()
        {
            float climbUpProgress = _animator.GetFloat("ClimbUpProgress");
            Vector3 currentForward = Vector3.Lerp(climbStartForward, dragonTransform.forward, climbUpProgress);
            _transform.forward = currentForward;

            if (climbUpProgress > 0.99f)
            {
                _loco.StartCoroutine(FixPlayerPosition(0.5f));
                Transform bone08 = Utility.RecursiveFind(dragonTransform, dragonNeckName);
                _transform.SetParent(bone08);
                _dragonGO.GetComponent<DragonController>().SetMounted(true);

                _loco.SetProcessor<OnDragonProcessor>(dragonTransform);
            }
        }
    }
}