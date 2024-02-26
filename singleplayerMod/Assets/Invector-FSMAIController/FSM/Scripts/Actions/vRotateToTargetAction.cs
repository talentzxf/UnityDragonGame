namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("Requires a CurrentTarget", UnityEditor.MessageType.Info)]
#endif
    public class vRotateToTargetAction : vStateAction
    {
        public override string categoryName
        {
            get { return "Movement/"; }
        }
        public override string defaultName
        {
            get { return "Rotate To Target"; }
        }
        public bool onlyIfIsInLineOfSight = true;
        public override void DoAction(vIFSMBehaviourController fsmBehaviour, vFSMComponentExecutionType executionType = vFSMComponentExecutionType.OnStateUpdate)
        {
            if (fsmBehaviour != null && fsmBehaviour.aiController.currentTarget.transform && (!onlyIfIsInLineOfSight||fsmBehaviour.aiController.targetInLineOfSight))
                fsmBehaviour.aiController.RotateTo(fsmBehaviour.aiController.lastTargetPosition-fsmBehaviour.transform.position);
        }
    }
}