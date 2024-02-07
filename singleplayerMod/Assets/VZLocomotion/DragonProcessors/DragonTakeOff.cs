using UnityEngine;
using VZLocomotion.DragonProcessors;

public class DragonTakeOff : AnimatedCharactorLocomotionProcessor
{
    private string rootBonePath = "Armature/Bone";
    private int takeOff = Animator.StringToHash("TakeOff");
    private Transform boneRoot;

    public override void OnActive(params object[] parameters)
    {
        base.OnActive(parameters);

        boneRoot = _transform.Find(rootBonePath);

        _animator.SetTrigger(takeOff);
    }

    public override void Update()
    {
        float takeOffProgress = _animator.GetFloat("TakeOffProgress");
        Debug.Log("TakeOff Progress:" + takeOffProgress);
        
        if (takeOffProgress > 0.99)
        {
            _transform.position = boneRoot.position;
            
            Debug.Log("Set dragon transform to:" + _transform.position + " ," + _animator.GetCurrentAnimatorClipInfo(0)[0].clip.name);
            _loco.SetProcessor<DragonFlying>(boneRoot);
        }
    }
}