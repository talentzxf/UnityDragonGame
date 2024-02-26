using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("This is a TriggerSound Action", UnityEditor.MessageType.Info)]
#endif
    public class vTriggerSoundAction : vStateAction
    {       
       public override string categoryName
        {
            get { return "Generic/"; }
        }
        public override string defaultName
        {
            get { return "Trigger Sound"; }
        }
        public vTriggerSoundAction()
        {
            executionType = vFSMComponentExecutionType.OnStateEnter;
        }

        //public AudioSource source; //use to Example 1

        public AudioClip[] clips;
        public float minVolume=0.5f, maxVolume =1;

        public override void DoAction(vIFSMBehaviourController fsmBehaviour, vFSMComponentExecutionType executionType = vFSMComponentExecutionType.OnStateUpdate)
        {
            #region Example 1
            if (executionType == vFSMComponentExecutionType.OnStateEnter)
            {
                AudioSource.PlayClipAtPoint(clips[Random.Range(0, clips.Length)], fsmBehaviour.transform.position, Random.Range(minVolume, maxVolume));
            }
            #endregion

            #region Example 2
            //if (executionType == vFSMComponentExecutionType.OnStateEnter)
            //{
            //    var _source = Instantiate(source, fsmBehaviour.transform.position, Quaternion.identity);
            //    _source.volume = Random.Range(minVolume, maxVolume);
            //    _source.PlayOneShot(clips[Random.Range(0, clips.Length)]);
            //}
            #endregion
        }
    }
}