using System;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class AbstractNameTag : NetworkBehaviour
{
    private TextMeshProUGUI nameText;
    private string _userId;
    private Camera mainCamera;

    private string _postFix;

    private string _originalString;

    public void SetPostFix(string postFix)
    {
        if (postFix != null)
            nameText.text = _originalString + postFix;
        else
            nameText.text = _originalString;
    }

    private void Sync()
    {
        if (isHide)
            return;

        if (nameText != null)
        {
            var targetPoint = GetTextPosition();

            bool isInScreen = Utility.IsVisibleOnScreen(mainCamera, targetPoint, out Vector3 screenPoint);
            if (isInScreen)
            {
                nameText.enabled = true;
                nameText.transform.position = screenPoint;
            }
            else
            {
                nameText.enabled = false;
            }
        }
    }

    public override void FixedUpdateNetwork()
    {
        Sync();
    }

    private void Update()
    {
        Sync();
    }

    private void LateUpdate()
    {
        Sync();
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        if (isHide)
            return;

        Destroy(nameText);
    }

    private bool isHide = false;

    protected abstract void InitOnSpawn();
    protected abstract bool HideLocalObjectName();

    protected abstract string GetObjectName();

    protected abstract Vector3 GetTextPosition();

    public override void Spawned()  
    {
        InitOnSpawn();
        var no = GetComponent<NetworkObject>();
        if (HideLocalObjectName() && no.InputAuthority == Runner.LocalPlayer)
        {
            isHide = true;
            return;
        }

        mainCamera = Camera.main;

        GameObject canvasGO = null;
        var canvasComponent = FindObjectOfType<Canvas>();
        if (canvasComponent == null) // Create Cavnas
        {
            canvasGO = new GameObject("Canvas");
            canvasGO.AddComponent<Canvas>();
            SceneManager.MoveGameObjectToScene(canvasGO, SceneManager.GetActiveScene());
        }
        else
        {
            canvasGO = canvasComponent.gameObject;
        }

        var nameTextGO = new GameObject("NameTag");
        nameTextGO.transform.parent = canvasGO.transform;
        nameText = nameTextGO.AddComponent<TextMeshProUGUI>();
        nameText.fontSize = 20;
        nameText.alignment = TextAlignmentOptions.Center;

        nameText.text = GetObjectName();

        _originalString = nameText.text;
    }
}