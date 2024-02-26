using Invector;
using Invector.vEventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum TargetPriority
{
    None = 0, Minimal = 1, Medium = 2, High = 3, Maximum = 4,
}
public partial class vAITargetInfo : MonoBehaviour
{
    public string targetTag;
    public TargetPriority priority;
    public vIHealthController healthController;
    private void Awake()
    {
        healthController = GetComponent<vIHealthController>();
    }
}