using System;
using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("This is a IsInShotAngleDecision decision", UnityEditor.MessageType.Info)]
#endif
    public class IsInShotAngleDecision : vStateDecision
    {
		public override string categoryName
        {
            get { return "Combat/"; }
        }

        public override string defaultName
        {
            get { return "IsInShotAngleDecision"; }
        }
        public override Type requiredType => typeof(vIControlAIShooter);
        public override bool Decide(vIFSMBehaviourController fsmBehaviour)
        {
            if (fsmBehaviour.aiController is vIControlAIShooter)
            {
                return (fsmBehaviour.aiController as vIControlAIShooter).IsInShotAngle;
            }
            return false;
        }
    }
}
