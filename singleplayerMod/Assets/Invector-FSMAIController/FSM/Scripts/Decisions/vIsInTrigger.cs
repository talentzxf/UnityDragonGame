using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("This is a vIsInTrigger decision", UnityEditor.MessageType.Info)]
#endif
    public class vIsInTrigger : vStateDecision
    {
		public override string categoryName
        {
            get { return "Trigger/"; }
        }

        public override string defaultName
        {
            get { return "vIsInTrigger"; }
        }
        [vToggleOption("Method","Compare tag","Compare name")]
        public bool useName = false;
        public string compareTrigger;
        public override bool Decide(vIFSMBehaviourController fsmBehaviour)
        {
           
            return useName? fsmBehaviour.aiController.IsInTriggerWithName(compareTrigger): fsmBehaviour.aiController.IsInTriggerWithTag(compareTrigger);
        }
    }
}
