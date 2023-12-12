using UnityEngine;

public class DragonMounted : AnimatedCharactorLocomotionProcessor
{
    int takeOff = Animator.StringToHash("TakeOff");

    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _animator.SetTrigger(takeOff);
            _loco.SetProcessor<DragonTakeOff>();
        }
    }
}