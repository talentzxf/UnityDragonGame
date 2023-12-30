using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GemMovement : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float duration;
    
    void Start()
    {
        Vector3 targetPosition = transform.position + 5 * transform.right;
        transform.DOMove(targetPosition, 2).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }
}
