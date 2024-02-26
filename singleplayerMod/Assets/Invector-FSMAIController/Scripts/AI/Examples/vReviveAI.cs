using System.Collections;
using UnityEngine;
namespace Invector.vCharacterController.AI
{
    //[vClassHeader("REVIVE AI", openClose = false)]
    //public class vReviveAI : vMonoBehaviour
    //{
    //    public float reviveDelay = 5f;
    //    public UnityEngine.Events.UnityEvent onRevive;
    //    vControlAI controlAI;
    //    bool inRevive;

    //    private void Start()
    //    {
    //        controlAI = GetComponent<vControlAI>();
    //        ///create event for OnDead 
    //        controlAI.onDead.AddListener((GameObject g) => { ReviveAI(); });
    //    }
    //    /// <summary>
    //    /// Revive AI method
    //    /// </summary>
    //    public void ReviveAI()
    //    {
    //        ///Start ReviveCoroutine
    //        if (!inRevive) StartCoroutine(ReviveCoroutine());

    //    }

    //    IEnumerator ReviveCoroutine()
    //    {
    //        inRevive = true;
    //        yield return new WaitForSeconds(reviveDelay);
    //        controlAI.ResetHealth();
    //        controlAI._capsuleCollider.enabled = true;
    //        controlAI._rigidbody.isKinematic = false;
    //        controlAI.animator.SetBool("isDead", false);
    //        controlAI.triggerDieBehaviour = false;
    //        inRevive = false;
    //    }
    //}
}