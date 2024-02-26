namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("Verify if your CurrentTarget is Attacking you", UnityEditor.MessageType.Info)]
#endif
    public class vTargetIsAttacking : vStateDecision
    {
        public override string categoryName
        {
            get { return "Detection/"; }
        }
        public override string defaultName
        {
            get { return "Target Is Attacking?"; }
        }

        public override bool Decide(vIFSMBehaviourController fsmBehaviour)
        {
            /// why we need that?
            throw new System.NotImplementedException("TODO");
        }
    }
}