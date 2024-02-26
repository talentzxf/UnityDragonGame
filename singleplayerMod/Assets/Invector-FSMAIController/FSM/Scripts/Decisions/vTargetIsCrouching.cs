namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("Verify if your CurrentTarget is Crouching", UnityEditor.MessageType.Info)]
#endif
    public class vTargetIsCrouching : vStateDecision
    {
        public override string categoryName
        {
            get { return "Detection/"; }
        }
        public override string defaultName
        {
            get { return "Target Is Crouching?"; }
        }

        public override bool Decide(vIFSMBehaviourController fsmBehaviour)
        {
            throw new System.NotImplementedException("TODO");          
        }
    }
}