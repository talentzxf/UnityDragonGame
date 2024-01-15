using System.Collections.Generic;
using Fusion;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class AbstractNameTag : NetworkBehaviour
{
    [SerializeField] private GameObject nameTagPrefab;

    private TextMeshProUGUI nameText;
    private string _userId;
    private Camera mainCamera;

    private string _postFix;

    private string _originalString;

    public void SetTextColor(Color textColor)
    {
        nameText.color = textColor;
    }

    public void Hide()
    {
        isHide = true;
        nameText.gameObject.SetActive(false);
        nameText.enabled = false;
    }
    
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

    public abstract Vector3 GetTextPosition();

    private void Awake()
    {
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

        var nameTextGO = Instantiate(nameTagPrefab, canvasGO.transform, true);
        nameText = nameTextGO.GetComponent<TextMeshProUGUI>();
        nameText.fontSize = 20;
        nameText.alignment = TextAlignmentOptions.Center;
    }
    
    public override void Spawned()  
    {
        InitOnSpawn();
        var no = GetComponent<NetworkObject>();
        if (HideLocalObjectName() && no.InputAuthority == Runner.LocalPlayer)
        {
            isHide = true;
            return;
        }

        nameText.text = GetObjectName();

        _originalString = nameText.text;
    }

    private Dictionary<string, MMFeedbacks> _effects = new();
    public void PlayEffect(string effectName)
    {
        if (!_effects.TryGetValue(effectName, out MMFeedbacks feedback))
        {
            feedback = nameText.transform.Find(effectName)?.GetComponent<MMFeedbacks>();
            if (feedback != null)
            {
                _effects.Add(effectName, feedback);
            }
        }
        
        feedback?.PlayFeedbacks();
    }
}