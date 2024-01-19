using System;
using System.Collections;
using System.Collections.Generic;
using DragonGameNetworkProject;
using Fusion;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

class ResultUIInfo: IComparable<ResultUIInfo>
{
    private RenderTexture _renderTexture;
    private PlayerRef _playerRef;
    private int _points;
    
    public RenderTexture Texture =>_renderTexture;
    public PlayerRef PlayerRef => _playerRef;
    public int Points => _points;

    public ResultUIInfo(RenderTexture renderTexture, PlayerRef playerRef, int points)
    {
        _renderTexture = renderTexture;
        _playerRef = playerRef;
        _points = points;
    }

    public int CompareTo(ResultUIInfo obj)
    {
        return obj._points.CompareTo(_points);
    }
}

public class GameResultUI : MonoBehaviour
{
    private UIDocument uiDoc;
    private void Awake()
    {
        uiDoc = GetComponent<UIDocument>();
    }

    // Start is called before the first frame update
    void Start()
    {
        GameTimer.Instance.onGameCompleted.AddListener(() =>
        {
            uiDoc.enabled = true;

            var sibilingUIDocuments = transform.parent.GetComponentsInChildren<UIDocument>();
            foreach (var uiDocument in sibilingUIDocuments)
            {
                if (uiDocument == uiDoc)
                    continue;

                uiDocument.enabled = false;
            }

            SetupResultUIAvatars(PrepareUI.Instance.Runner);
        });
    }

    void SetupResultUIAvatars(NetworkRunner runner)
    {
        var resultUIs = new SortedSet<ResultUIInfo>();
        foreach (var controller in PrepareUI.Instance.Controllers)
        {
            var avatarTexture = controller.TakeAvatarSnapshot();
            var playerRef = controller.gameObject.GetComponent<NetworkObject>().StateAuthority;

            var points = Bonus.Instance.GetCoinCount(playerRef);

            resultUIs.Add(new ResultUIInfo(avatarTexture, playerRef, points));
        }

        var leftBar = uiDoc.rootVisualElement.Q<VisualElement>("Left");
        foreach (var uiInfo in resultUIs)
        {
            Utility.SetupAvatarUI(runner, leftBar, uiInfo.PlayerRef, uiInfo.Texture);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
