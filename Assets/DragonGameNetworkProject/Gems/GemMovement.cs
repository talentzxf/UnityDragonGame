using System.Collections.Generic;
using DG.Tweening;
using Fusion;
using Unity.VisualScripting;
using UnityEngine;

public class GemMovement : NetworkBehaviour
{
    [SerializeField] private float xDistance = 5f;
    [SerializeField] private float rotationDuration = 5f;
    [SerializeField] private int value = 1;
    [SerializeField] private List<Material> materialList;
    [Networked] private int materIdx { set; get; } = -1;
    [Networked] private bool isVisible { get; set; } = true;

    private MeshRenderer meshRenderer;
    private Collider collider;

    private ChangeDetector _changeDetector;

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            base.Spawned();
            Vector3 targetPosition = transform.position + xDistance * transform.right;
            transform.DOMove(targetPosition, 2).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);

            transform.DOLocalRotate(new Vector3(0f, 0.0f, 360.0f), rotationDuration, RotateMode.LocalAxisAdd)
                .SetLoops(-1, LoopType.Restart)
                .SetEase(Ease.Linear);

            // Host randomly pick up a material;
            materIdx = Random.Range(0, materialList.Count);
        }
        
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        meshRenderer = GetComponent<MeshRenderer>();
        collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (HasStateAuthority)
        {
            if (other.tag == "Player")
            {
                NetworkObject no = other.GetComponentInParent<NetworkObject>();
                if (no == null)
                    return;

                string msg = "Player:" + Runner.GetPlayerUserId(no.InputAuthority) + " Get " + value + " points!";
                UIController.Instance.ShowGameMsg(msg);
                Bonus.Instance.Add(no.StateAuthority, value);

                isVisible = false;
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
        
        foreach (var change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(isVisible):
                    meshRenderer.enabled = isVisible;
                    collider.enabled = isVisible;
                    break;
            }
        }
    }
}