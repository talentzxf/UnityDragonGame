using System.Collections.Generic;
using DG.Tweening;
using ExitGames.Client.Photon.StructWrapping;
using Fusion;
using MoreMountains.Feedbacks;
using Unity.VisualScripting;
using UnityEngine;

// enum GemStatus
// {
//     IsActive,
//     IsDisappearing,
//     Disappeared
// }

public class GemMovement : NetworkBehaviour
{
    // Forward child collider OnTriggerEnter to parent.
    class ChildColliderReaction : MonoBehaviour
    {
        public GemMovement parentMovement;
    
        private void OnTriggerEnter(Collider other)
        {
            parentMovement.OnTriggerEnter(other);
        }
    }

    [SerializeField] private float xDistance = 5f;
    [SerializeField] private float rotationDuration = 5f;
    [SerializeField] private int value = 1;
    [SerializeField] private List<Material> materialList;
    [Networked] private int materIdx { set; get; } = -1;

    private MeshRenderer meshRenderer;
    private Collider collider;

    private ChangeDetector _changeDetector;
    [Networked] private Vector3 targetPosition { get; set; }
    [Networked] private Vector3 startPosition { get; set; }

    private MMFeedbacks _mmFeedbacks;
    private MMFeedbackScale scaleFeedBack;
    // [Networked] private GemStatus status { get; set; } = GemStatus.IsActive;
    
    [Networked] private bool hasDisappeared { get; set; }
    
    public override void Spawned()
    {
        if (Runner.IsSharedModeMasterClient || Runner.IsSinglePlayer)
        {
            base.Spawned();

            startPosition = transform.position;
            targetPosition = transform.position + xDistance * transform.right;

            // Host randomly pick up a material;
            materIdx = Random.Range(0, materialList.Count);
        }
        
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        collider = GetComponentInChildren<Collider>();
        
        NetworkEventsHandler.SelectedAsMasterClient.AddListener(() =>
        {
            StartMove();
        });

        _mmFeedbacks = GetComponent<MMFeedbacks>();
        foreach (var feedback in _mmFeedbacks.Feedbacks)
        {
            if (feedback.IsType<MMFeedbackScale>())
            {
                scaleFeedBack = feedback as MMFeedbackScale;
            }
        }
        
        ChildColliderReaction childColliderReaction = collider.AddComponent<ChildColliderReaction>();
        childColliderReaction.parentMovement = this;
    }

    private void StartMove()
    {
        if (Runner.IsSharedModeMasterClient || Runner.IsSinglePlayer)
        {
            transform.position = startPosition;
            
            transform.DOMove(targetPosition, 2).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);

            transform.DOLocalRotate(new Vector3(0f, 360.0f, 0.0f), rotationDuration, RotateMode.LocalAxisAdd)
                .SetLoops(-1, LoopType.Restart)
                .SetEase(Ease.Linear);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void HideGemRpc()
    {
        hasDisappeared = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasDisappeared) // Prevent re-entry.
        {
            return;
        }
        
        // if (HasStateAuthority)
        {
            if (other.tag == "Player")
            {
                NetworkObject no = other.GetComponentInParent<NetworkObject>();
                if (no == null)
                    return;

                if (no.HasStateAuthority)
                {
                    string msg = "Player:" + Runner.GetPlayerUserId(no.InputAuthority) + " Get " + value + " points!";
                    UIController.Instance.ShowGameMsg(msg);
                    Bonus.Instance.AddPlayerCoinRpc(no.StateAuthority, value);

                    collider.enabled = false;
                    _mmFeedbacks.PlayFeedbacks();
                    
                    HideGemRpc(); // Tell the state authority, this gem is disabled.
                }
            }
        }
    }
    
    public void Update()
    {
        if (_changeDetector == null) // Game has not started.
            return;

        if (materIdx != -1 && meshRenderer != null)
        {
            meshRenderer.material = materialList[materIdx];
        }

        if (hasDisappeared)
        {
            if (scaleFeedBack == null || !scaleFeedBack.FeedbackPlaying)
            {
                collider.enabled = false;
                meshRenderer.enabled = false;
            }
        }

    }
}