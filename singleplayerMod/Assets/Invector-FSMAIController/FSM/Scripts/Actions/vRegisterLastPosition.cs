using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("This is a vRegisterLastPosition Action", UnityEditor.MessageType.Info)]
#endif
    public class vRegisterLastPosition : vStateAction
    {       
       public override string categoryName
        {
            get { return "Movement/"; }
        }
        public override string defaultName
        {
            get { return "Set Start Position"; }
        }

        public override void DoAction(vIFSMBehaviourController fsmBehaviour, vFSMComponentExecutionType executionType = vFSMComponentExecutionType.OnStateUpdate)
        {
            fsmBehaviour.aiController.selfStartPosition = fsmBehaviour.aiController.transform.position;
        }
    }
}