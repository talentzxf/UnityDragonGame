using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NameTag : NetworkBehaviour
{
    public string NameTagName = "NameTag";

    private Transform avatarTransform;
    private TextMeshPro nameText;
    private string _userId;
    private float avatarHeight = 1.6f;
    

    void Update()
    {
        if (nameText != null)
        {
            nameText.transform.position = avatarTransform.position + avatarHeight * avatarTransform.up;
            nameText.transform.LookAt(Camera.main.transform);
        }
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        Destroy(nameText);
    }

    public override void Spawned()
    {
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
        nameText = nameTextGO.AddComponent<TextMeshPro>();
        nameText.fontSize = 1;
        nameText.alignment = TextAlignmentOptions.Center;

        PlayerMovementNetwork playerMovementNetwork = gameObject.GetComponentInChildren<PlayerMovementNetwork>();
        if (nameText != null && playerMovementNetwork != null)
        {
            avatarTransform = playerMovementNetwork.transform;
            var no = GetComponent<NetworkObject>();
            var runner = no.Runner;
            var userId = runner.GetPlayerUserId(no.InputAuthority);
            nameText.text = userId;

            var _cc = GetComponentInChildren<CharacterController>();
            if (_cc)
            {
                avatarHeight = _cc.height;                
            }            
        }
    }
}