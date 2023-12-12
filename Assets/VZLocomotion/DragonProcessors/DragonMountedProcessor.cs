using UnityEngine;

namespace DefaultNamespace.VZLocomotion.DragonProcessors
{
    public class DragonMountedProcessor: ICharacterLocomotionProcessor
    {
        private Animator _animator;
        
        int takeOff = Animator.StringToHash("TakeOff");
        
        public override void Setup(CharacterLocomotion locomotion)
        {
            base.Setup(locomotion);
            _animator = _go.GetComponent<Animator>();
        }

        public override void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _animator.SetTrigger(takeOff);
            }
        }
    }
}