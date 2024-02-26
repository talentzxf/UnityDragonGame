using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("This is a vIsClosestListenerToNoise decision", UnityEditor.MessageType.Info)]
#endif
    public class vIsClosestListenerToNoise : vStateDecision
    {
		public override string categoryName
        {
            get { return "Noise/"; }
        }

        public override string defaultName
        {
            get { return "IsClosestListenerToNoise?"; }
        }

        public override bool Decide(vIFSMBehaviourController fsmBehaviour)
        {
            var noiseListener = fsmBehaviour.aiController.GetAIComponent<vAINoiseListener>();
            if(noiseListener!=null)
            {
                if(noiseListener.LastListenedNoise!=null)
                    return noiseListener.IsClosestListenerToNoise(noiseListener.LastListenedNoise);
            }
            return false;
        }
    }
}
