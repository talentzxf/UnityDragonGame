using System;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NameTag : NetworkBehaviour
{
    private Transform avatarTransform;
    private TextMeshProUGUI nameText;
    private string _userId;
    private float avatarHeight = 1.6f;

    private void Sync()
    {
        if (isLocal)
            return;
        
        if (nameText != null)
        {
            var targetPoint = avatarTransform.position + (avatarHeight * 1.1f) * avatarTransform.up;
            
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(targetPoint);

            Vector3 cameraRay = targetPoint - Camera.main.transform.position;
            
            bool isOutOfScreen = (screenPoint.x < 0 || screenPoint.x > Screen.width || screenPoint.y < 0 || screenPoint.y > Screen.height);
            if (Vector3.Dot(Camera.main.transform.forward, cameraRay) < 0) // The point is in the back of the camera.
            {
                isOutOfScreen = true;
            }

            if (!isOutOfScreen)
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
        if (isLocal)
            return;
        
        Destroy(nameText);
    }

    private bool isLocal = false;
    public override void Spawned()
    {
        var no = GetComponent<NetworkObject>();
        if (no.InputAuthority == Runner.LocalPlayer)
        {
            isLocal = true;
            return;
        }
        
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

        CharacterController cc = gameObject.GetComponentInChildren<CharacterController>();
        if (nameText != null && cc != null)
        {
            avatarTransform = cc.transform;
            var runner = no.Runner;
            var userId = runner.GetPlayerUserId(no.InputAuthority);
            nameText.text = userId;
            avatarHeight = cc.height;
        }
    }
}