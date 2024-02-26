namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("Use it to make the AI Controller chase a target * Requires a CurrentTarget *", UnityEditor.MessageType.Info)]
#endif
    public class vGoToTarget : vStateAction
    {
        public override string categoryName
        {
            get { return "Movement/"; }
        }
        public override string defaultName
        {
            get { return "Chase Target"; }
        }

        public bool useStrafeMovement = false;
        [vHideInInspector("useStrafeMovement")]
        public bool updateRotationInStrafe = false;
        public vAIMovementSpeed speed = vAIMovementSpeed.Walking;

        public override void DoAction(vIFSMBehaviourController fsmBehaviour, vFSMComponentExecutionType executionType = vFSMComponentExecutionType.OnStateUpdate)
        {
            if (fsmBehaviour.aiController == null) return;
            if (executionType == vFSMComponentExecutionType.OnStateEnter) fsmBehaviour.aiController.ForceUpdatePath(2f);

            if (useStrafeMovement)
            {
                if (updateRotationInStrafe)
                {
                    var dir = fsmBehaviour.aiController.targetInLineOfSight ? fsmBehaviour.aiController.lastTargetPosition - fsmBehaviour.transform.position : fsmBehaviour.aiController.desiredVelocity;
                    fsmBehaviour.aiController.StrafeMoveTo(fsmBehaviour.aiController.lastTargetPosition, dir, speed);
                }
                else fsmBehaviour.aiController.StrafeMoveTo(fsmBehaviour.aiController.lastTargetPosition,  speed);
            }
            else fsmBehaviour.aiController.MoveTo(fsmBehaviour.aiController.lastTargetPosition, speed);
        }
    }
}