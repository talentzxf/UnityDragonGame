using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("Requires a ControlAIShooter", UnityEditor.MessageType.Info)]
#endif
    public class vAIShooterAttack : vStateAction
    {
        public override string categoryName
        {
            get { return "Combat/"; }
        }
        public override string defaultName
        {
            get { return "Trigger ShooterAttack"; }
        }

        public vAIShooterAttack()
        {
            executionType = vFSMComponentExecutionType.OnStateEnter | vFSMComponentExecutionType.OnStateExit | vFSMComponentExecutionType.OnStateUpdate;
        }
     
        [vHelpBox("Use this to ignore attack time")]
        public bool forceCanAttack;
        [Tooltip("This action will check if aim is enabled using vAnimatorTag with  (Upperbody Pose) tag on layer setted")]
        public int aimLayer = 4;
        [Tooltip("The shot rountine just will run when Aim angle is in Max Angle To Shot (Inspector of vControlAIShooter>ShooterSettings")]
        public bool onlyShotWhenInAngle;
        public bool debug;
        public override void DoAction(vIFSMBehaviourController fsmBehaviour, vFSMComponentExecutionType executionType = vFSMComponentExecutionType.OnStateUpdate)
        {
            if (fsmBehaviour.aiController is vIControlAIShooter)
            {
                ControlAttack(fsmBehaviour, fsmBehaviour.aiController as vIControlAIShooter,executionType);
            }
        }

        protected virtual void ControlAttack(vIFSMBehaviourController fsmBehaviour, vIControlAIShooter combat, vFSMComponentExecutionType executionType = vFSMComponentExecutionType.OnStateUpdate)
        {
            switch (executionType)
            {
                case vFSMComponentExecutionType.OnStateEnter:
                    InitAttack(combat);
                    break;
                case vFSMComponentExecutionType.OnStateUpdate:
                    HandleAttack(fsmBehaviour, combat);
                    break;
                case vFSMComponentExecutionType.OnStateExit:
                    FinishAttack(combat);
                    break;
            }
        }

        protected virtual void InitAttack(vIControlAIShooter combat)
        {
            combat.isInCombat = true;
            combat.InitAttackTime();
        }

        protected virtual void HandleAttack(vIFSMBehaviourController fsmBehaviour, vIControlAIShooter combat)
        {
            combat.AimToTarget(.2f);
            bool validAimAngle = onlyShotWhenInAngle ? combat.IsInShotAngle : true;
            if (!validAimAngle || !combat.isAiming || !combat.animatorStateInfos.HasTag("Upperbody Pose") || combat.animator.IsInTransition(aimLayer) || combat.animator.GetCurrentAnimatorStateInfo(aimLayer).normalizedTime < 0.9f) return;
            if (debug)
            {
                Debug.Log("Trigger Shooter Attack");
                fsmBehaviour.SendDebug("Trigger Shooter Attack", this);
            }
            combat.Attack(forceCanAttack: forceCanAttack);
        }
      
        protected virtual void FinishAttack(vIControlAIShooter combat)
        {
            combat.isInCombat = false;
            combat.ResetAttackTime();
        }
    }
}