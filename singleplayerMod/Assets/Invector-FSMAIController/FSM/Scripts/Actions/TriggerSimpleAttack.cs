using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("Trigger an Attack Animation", UnityEditor.MessageType.Info)]
#endif
    public class TriggerSimpleAttack : vStateAction
    {
        public string animatorStateName;
        public float attackDistance = 1f;
        public float rotateToTargetSmooth = 5;
        public vAIMovementSpeed attackSpeed = vAIMovementSpeed.Walking;

        public override string categoryName
        {
            get { return "Combat/"; }
        }

        public override string defaultName
        {
            get { return "Trigger Generic Attack"; }
        }

        public TriggerSimpleAttack()
        {
            executionType = vFSMComponentExecutionType.OnStateUpdate | vFSMComponentExecutionType.OnStateEnter | vFSMComponentExecutionType.OnStateExit;
        }

        public override void DoAction(vIFSMBehaviourController fsmBehaviour, vFSMComponentExecutionType executionType = vFSMComponentExecutionType.OnStateUpdate)
        {

            Attack(fsmBehaviour.aiController as vIControlAICombat, executionType);
        }

        public virtual void Attack(vIControlAICombat aICombat, vFSMComponentExecutionType executionType = vFSMComponentExecutionType.OnStateUpdate)
        {
            if (executionType == vFSMComponentExecutionType.OnStateEnter)
            {
                aICombat.InitAttackTime();
            }

            if (aICombat != null && aICombat.currentTarget.transform)
            {
                var distance = aICombat.targetDistance;
                if (distance <= (attackDistance))
                {
                    if (aICombat.isMoving) aICombat.Stop();
                    aICombat.RotateTo(aICombat.currentTarget.transform.position - aICombat.transform.position);
                    Vector3 targetDirection = aICombat.currentTarget.transform.position - aICombat.transform.position;
                    targetDirection.y = 0;
                    aICombat.transform.rotation = Quaternion.Lerp(aICombat.transform.rotation, Quaternion.LookRotation(targetDirection, Vector3.up), Time.deltaTime * rotateToTargetSmooth);
                    if (!aICombat.isAttacking && aICombat.canAttack)
                    {
                        aICombat.animator.PlayInFixedTime(animatorStateName);
                        aICombat.InitAttackTime();
                    }
                }
                else
                {
                    aICombat.MoveTo(aICombat.currentTarget.transform.position, attackSpeed);
                }
            }
            if (executionType == vFSMComponentExecutionType.OnStateExit)
            {
                aICombat.ResetAttackTime();
            }
        }
    }
}