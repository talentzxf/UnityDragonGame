using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("This is a vPlayAnimationAction Action", UnityEditor.MessageType.Info)]
#endif
    public class vPlayAnimationAction : vStateAction
    {       
       public override string categoryName
        {
            get { return "Animator/"; }
        }
        public override string defaultName
        {
            get { return "Play Animation"; }
        }

        public string _animationState;
        public int _layer;
        public float crossfade=0.2f;
        public vPlayAnimationAction()
        {
            executionType = vFSMComponentExecutionType.OnStateEnter;
        }

        public override void DoAction(vIFSMBehaviourController fsmBehaviour, vFSMComponentExecutionType executionType = vFSMComponentExecutionType.OnStateUpdate)
        {
            fsmBehaviour.aiController.animator.CrossFadeInFixedTime(_animationState,crossfade, _layer);
        }
    }
}