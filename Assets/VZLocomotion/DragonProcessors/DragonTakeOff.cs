using UnityEngine;

public class DragonTakeOff : AnimatedCharactorLocomotionProcessor
{
    public override void Update()
    {
        float takeOffProgress = _animator.GetFloat("TakeOffProgress");

        Debug.Log("Take off progress:" + takeOffProgress + ", position.y" + _transform.position.y);

        if (takeOffProgress > 0.99)
        {
            Debug.Log("Has taken off now");
        }
    }
}