using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{

    /// <summary>
    /// In this example we're calling the method MoveToTargetExample from the <seealso cref="vControlAI_ExpandExample"/>
    /// It's just an example on how to expand the AI Controller by adding your own methods and variables
    /// </summary>


#if UNITY_EDITOR
    [vFSMHelpbox("This is just an example on how to create new Action", UnityEditor.MessageType.Info)]
#endif
    public class vMoveToTarget_Example : vStateAction
    {       
       public override string categoryName
        {
            get { return "Custom Example/"; }
        }
        public override string defaultName
        {
            get { return "MoveToTarget Example"; }
        }

        // set a custom speed to the controller
        public vAIMovementSpeed speed = vAIMovementSpeed.Walking;

        public override void DoAction(vIFSMBehaviourController fsmBehaviour, vFSMComponentExecutionType executionType = vFSMComponentExecutionType.OnStateUpdate)
        {
            Debug.Log("FSM Calling 'MoveToTargetExample' ");
            fsmBehaviour.aiController.MoveToTargetExample(speed);            
        }
    }
}