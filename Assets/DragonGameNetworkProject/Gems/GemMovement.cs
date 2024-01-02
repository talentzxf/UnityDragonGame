using System.Collections.Generic;
using DG.Tweening;
using Fusion;
using Unity.VisualScripting;
using UnityEngine;

struct RecordTransform : INetworkStruct
{
    public Vector3 position;
    public Quaternion rotation;

    public void Set(Transform transform)
    {
        position = transform.position;
        rotation = transform.rotation;
    }
}

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
    [Networked] private Vector3 targetPosition { get; set; }
    [Networked] private RecordTransform startTransform { get; set; }
    
    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            base.Spawned();

            startTransform.Set(transform);
            targetPosition = transform.position + xDistance * transform.right;
            StartMove();

            // Host randomly pick up a material;
            materIdx = Random.Range(0, materialList.Count);
        }
        
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        meshRenderer = GetComponent<MeshRenderer>();
        collider = GetComponent<Collider>();
        
        NetworkEventsHandler.HostMigrated.AddListener(() =>
        {
            transform.position = startTransform.position;
            StartMove();
        });
    }

    private void StartMove()
    {
        if (HasStateAuthority)
        {
            // transform.rotation = startTransform.rotation;
            // transform.position = startTransform.position;
            
            transform.DOMove(targetPosition, 2).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);

            transform.DOLocalRotate(new Vector3(0f, 0.0f, 360.0f), rotationDuration, RotateMode.LocalAxisAdd)
                .SetLoops(-1, LoopType.Restart)
                .SetEase(Ease.Linear);
        }
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