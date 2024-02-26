using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("This is a vAimToTargetAction Action", UnityEditor.MessageType.Info)]
#endif
    public class vAimToTargetAction : vStateAction
    {
        public override string categoryName
        {
            get { return "Combat/"; }
        }
        public override string defaultName
        {
            get { return "Aim To Target"; }
        }

        public vAimToTargetAction()
        {
            executionType =  vFSMComponentExecutionType.OnStateUpdate;
        }
        public bool onlyIfCanSeeTarget;
        [Tooltip("This action will check if aim is enabled using vAnimatorTag with  (Upperbody Pose) tag on layer setted")]
        public int aimLayer = 4;
        public override void DoAction(vIFSMBehaviourController fsmBehaviour, vFSMComponentExecutionType executionType = vFSMComponentExecutionType.OnStateUpdate)
        {
            if (fsmBehaviour.aiController is vIControlAIShooter)
            {
                ControlAttack(fsmBehaviour, fsmBehaviour.aiController as vIControlAIShooter, executionType);
            }
        }

        protected virtual void ControlAttack(vIFSMBehaviourController fsmBehaviour, vIControlAIShooter combat, vFSMComponentExecutionType executionType = vFSMComponentExecutionType.OnStateUpdate)
        {
            if(!onlyIfCanSeeTarget|| combat.targetInLineOfSight ) combat.AimToTarget(.1f);

        }
    }
}