using System.Collections.Generic;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("Requires a AINoiseListener attached to your AI Controller - Use it to make the AI go to the Noise position", UnityEditor.MessageType.Info)]
#endif
    public class vGoToNoisePosition : vStateAction
    {
        public bool findNewNoise = false;
        public bool specificType;
        [vHideInInspector("findNewNoise;specificType")]
        public List<string> noiseTypes;
        public bool lookToNoisePosition = true;

        public override string categoryName
        {
            get { return "Movement/Noise/"; }
        }
        public override string defaultName
        {
            get { return "Go To Noise Position"; }
        }

        public vAIMovementSpeed speed = vAIMovementSpeed.Walking;

        public override void DoAction(vIFSMBehaviourController fsmBehaviour, vFSMComponentExecutionType executionType = vFSMComponentExecutionType.OnStateUpdate)
        {
            if (fsmBehaviour.aiController != null)
            {
                if (fsmBehaviour.aiController.HasComponent<vAINoiseListener>())
                {

                    var noiseListener = fsmBehaviour.aiController.GetAIComponent<vAINoiseListener>();
                    vNoise noise = null;
                    if (findNewNoise)
                    {
                        if (specificType) noise = noiseListener.GetNearNoiseByTypes(noiseTypes);
                        else noise = noiseListener.GetNearNoise();
                    }
                    else noise = noiseListener.LastListenedNoise;
                    if (noise != null)
                    {
                        fsmBehaviour.aiController.MoveTo(noise.position,speed);
                        if (lookToNoisePosition) fsmBehaviour.aiController.LookTo(noise.position, offsetLookHeight: 0);
                    }
                }
            }
        }
    }
}
