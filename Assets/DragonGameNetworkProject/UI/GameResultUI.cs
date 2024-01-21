using System;
using System.Collections.Generic;
using System.Linq;
using DragonGameNetworkProject;
using Fusion;
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
    private Label resultLabel;
    private Label coinCountLabel;
    private Label winnerLabel;
    private Button quitBtn;

    [SerializeField] private Texture medalTexture;
    
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
            
            resultLabel = uiDoc.rootVisualElement.Q<Label>("Result");
            winnerLabel = uiDoc.rootVisualElement.Q<Label>("Winner");
            coinCountLabel = uiDoc.rootVisualElement.Q<Label>("CoinCount");
            quitBtn = uiDoc.rootVisualElement.Q<Button>("QuitBtn");

            quitBtn.clicked += Application.Quit;

            SetupResultUIAvatars(PrepareUI.Instance.Runner);
        });
    }

    void SetupResultUIAvatars(NetworkRunner runner)
    {
        Debug.Log("Begin to prepare UI");
        var resultUIs = new List<ResultUIInfo>();
        ResultUIInfo localPlayerInfo = null;
        foreach (var controller in PrepareUI.Instance.Controllers)
        {
            var avatarTexture = controller.TakeAvatarSnapshot();
            var playerRef = controller.gameObject.GetComponent<NetworkObject>().StateAuthority;
            var points = Bonus.Instance.GetCoinCount(playerRef);
            
            Debug.Log("Getting UI for:" + runner.GetPlayerUserId(playerRef) );

            var playerInfo = new ResultUIInfo(avatarTexture, playerRef, points);
            if (playerRef == runner.LocalPlayer)
            {
                localPlayerInfo = playerInfo;
            }

            resultUIs.Add(playerInfo);
        }
        
        resultUIs.Sort();

        var winner = resultUIs.FirstOrDefault();
        var leftBar = uiDoc.rootVisualElement.Q<VisualElement>("Left");
        foreach (var uiInfo in resultUIs)
        {
            var playerEle = Utility.SetupAvatarUI(runner, leftBar, uiInfo.PlayerRef, uiInfo.Texture);
            
            Debug.Log("Getting uiInfo for:" + runner.GetPlayerUserId(uiInfo.PlayerRef) );

            var label = new Label();
            label.text = "Score:" + uiInfo.Points;
            playerEle.Add(label);

            if (uiInfo.PlayerRef == winner.PlayerRef)
            {
                var medalImg = Utility.CreateImageInAvatarView(medalTexture);
                playerEle.Add(medalImg);
            }
        }
        
        Debug.Log("We have:" + PrepareUI.Instance.Controllers.Count + " players" );

        if (runner.LocalPlayer == winner.PlayerRef)
        {
            resultLabel.text = "You Win!";
        }
        else
        {
            resultLabel.text = "You Lose!";
        }

        winnerLabel.text = $"Winner is: {runner.GetPlayerUserId(winner.PlayerRef)}";
        coinCountLabel.text = $"You got: {localPlayerInfo?.Points??0} points!";
    }
}
