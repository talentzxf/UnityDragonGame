using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("This is a vAnimatorSetTrigger Action", UnityEditor.MessageType.Info)]
#endif
    public class vAnimatorSetTrigger : vStateAction
    {       
       public override string categoryName
        {
            get { return "Animator/"; }
        }
        public override string defaultName
        {
            get { return "vAnimatorSetTrigger"; }
        }
        public string trigger;
        [vToggleOption("Method","Set","Reset")]
        public bool reset;
        public override void DoAction(vIFSMBehaviourController fsmBehaviour, vFSMComponentExecutionType executionType = vFSMComponentExecutionType.OnStateUpdate)
        {
           if(reset) fsmBehaviour.aiController.animator.ResetTrigger(trigger);else fsmBehaviour.aiController.animator.SetTrigger(trigger);
            //TO DO
        }
    }
}