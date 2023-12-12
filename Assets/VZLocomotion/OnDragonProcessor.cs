using UnityEngine;

namespace DefaultNamespace.VZLocomotion
{
    public class OnDragonProcessor : ICharacterLocomotionProcessor
    {
        private CharacterController _character;
        private Animator _animator;

        private int _climbdown = Animator.StringToHash("climbDown");
        private Transform _dragonTransform;
        private GameObject _dragonGO;

        public override void OnActive(params object[] parameters)
        {
            _dragonTransform = parameters[0] as Transform;
            _dragonGO = _dragonTransform.gameObject;
        }

        public override void Setup(CharacterLocomotion locomotion)
        {
            base.Setup(locomotion);
            _character = _go.GetComponent<CharacterController>();
            _animator = _go.GetComponent<Animator>();
        }

        public override void Update()
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                Debug.Log("Clicked M, climbing down");
                _character.enabled = true;
                _animator.SetTrigger(_climbdown);
                _transform.SetParent(null);
                
                _dragonGO.GetComponent<CharacterLocomotion>().SetProcessor<DragonIdleProcessor>();
            }
        }
    }
}