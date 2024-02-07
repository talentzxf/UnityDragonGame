using JetBrains.Annotations;
using UnityEngine;

namespace DragonGameNetworkProject
{
    public abstract class AbstractMovement: MonoBehaviour
    {
        protected MecanimAnimator mecanimAnimator;
        protected Transform ccTransform;
        public CharacterMovementController controller;
        protected Animator animator;
        
        public void Start()
        {
            controller = this.GetComponent<CharacterMovementController>();
            mecanimAnimator = controller.avatarGO.GetComponent<MecanimAnimator>();
            ccTransform = controller.avatarGO.transform;
            animator = controller.avatarGO.GetComponent<Animator>();
        }

        public virtual void OnAnimatorIK(int layerIndex)
        {
        }

        public abstract void OnEnterMovement();
        public abstract void OnLeaveMovement();

        public virtual bool CanLeaveMovement()
        {
            return true;
        }

        public virtual bool CanEnterMovement()
        {
            return true;
        }
    }

   
   
}

public static class TypeUtility
{
    public static bool IsType<T>(this object obj)
    {
        switch (obj)
        {
            case T _:
                return true;
            default:
                return false;
        }
    }
}