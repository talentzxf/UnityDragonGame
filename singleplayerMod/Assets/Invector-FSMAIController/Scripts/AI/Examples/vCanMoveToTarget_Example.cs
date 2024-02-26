using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR

    /// <summary>
    /// In this example we're verifying a bool from the script <seealso cref="vControlAI_ExpandExample"/>
    /// It's just an example on how to expand the AI Controller by adding your own methods and variables
    /// </summary>
    
    [vFSMHelpbox("This is just an example of a custom Decision", UnityEditor.MessageType.Info)]
#endif
    public class vCanMoveToTarget_Example : vStateDecision
    {
		public override string categoryName
        {
            get { return "Custom Example/"; }
        }

        public override string defaultName
        {
            get { return "Can MoveToTarget Example"; }
        }

        public override bool Decide(vIFSMBehaviourController fsmBehaviour)
        {
            // This custom decision that will verify the bool 'moveToTarget' and return if it's true or false
            return fsmBehaviour.aiController._moveToTarget;
        }
    }
}
