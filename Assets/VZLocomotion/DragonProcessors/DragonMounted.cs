using UnityEngine;

public class DragonMounted : AnimatedCharactorLocomotionProcessor
{
    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _loco.SetProcessor<DragonTakeOff>();
        }
    }
}