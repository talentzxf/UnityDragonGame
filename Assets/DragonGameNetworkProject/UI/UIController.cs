using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.UIElements;

enum MSGTYPE
{
    SYSTEM,
    GAME
}

struct Message
{
    public Message(MSGTYPE type, string text)
    {
        msgType = type;
        msgText = text;
    }

    private MSGTYPE msgType;
    private string msgText;

    public override string ToString()
    {
        return Enum.GetName(typeof(MSGTYPE), msgType) + ":" + msgText;
    }
}

public class UIController : MonoBehaviour
{
    private UIDocument uiDocument;

    private Label messageLabel;
    private VisualElement dragonControlUI;
    private ProgressBar speedBar;

    private Label bonusLabel;
    private Label playerListLabel;
    private Label timerLabel;
    private Label dragonControlPrompt;

    private Queue<Message> curMessages = new Queue<Message>();

    private NetworkRunner _runner;

    private static UIController _instance;

    public static UIController Instance => _instance;

    public void ShowSpeed(Vector3 curSpeed, float maxSpeed)
    {
        if (speedBar == null)
        {
            return;
        }

        float curSpeedMag = curSpeed.magnitude;

        speedBar.value = curSpeedMag;
        speedBar.lowValue = 0;
        speedBar.highValue = maxSpeed;

        speedBar.title = "Speed:" + curSpeedMag.ToString("F2");
    }

    public void ShowPrompt(string prompt)
    {
        dragonControlPrompt.visible = true;
        dragonControlPrompt.text = prompt;
    }

    public void ShowDragonControlUI()
    {
        dragonControlUI.style.visibility = Visibility.Visible;
    }
    
    public void HideDragonControlUI()
    {
        dragonControlUI.style.visibility = Visibility.Hidden;
        dragonControlPrompt.visible = false;
    }

    private void ActivateUiDocument()
    {
        uiDocument.gameObject.SetActive(true);
        messageLabel = uiDocument.rootVisualElement.Q<Label>("Messages");
        messageLabel.text = "";

        speedBar = uiDocument.rootVisualElement.Q<ProgressBar>("SpeedBar");
        dragonControlUI = uiDocument.rootVisualElement.Q<VisualElement>("DragonControlUI");

        bonusLabel = uiDocument.rootVisualElement.Q<Label>("Bonus");

        playerListLabel = uiDocument.rootVisualElement.Q<Label>("PlayerList");

        timerLabel = uiDocument.rootVisualElement.Q<Label>("Timer");
        dragonControlPrompt = uiDocument.rootVisualElement.Q<Label>("DragonControlPrompt");
        HideDragonControlUI();
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;

        // NetworkEventsHandler.LocalPlayerJoined.AddListener((runner, userId) =>
        // {
        //     _runner = runner;
        //     ActivateUiDocument();
        //     string localPlayerId = userId;
        //
        //     uiDocument.rootVisualElement.Q<Label>("LocalUserID").text = "UserID:" + localPlayerId;
        // });
        //
        // NetworkEventsHandler.ServerConnected.AddListener(runner =>
        // {
        //     ActivateUiDocument();
        //
        //     uiDocument.gameObject.SetActive(true);
        //
        //     string roomName = runner.SessionInfo.Name + "@" + runner.SessionInfo.Region;
        //     uiDocument.rootVisualElement.Q<Label>("RoomName").text = "Room:" + roomName;
        //
        //     this._runner = runner;
        //
        //     ShowSysMsg("Connected to server.");
        // });

        NetworkEventsHandler.PlayerJoined.AddListener((runner, userId) =>
        {
            _runner = runner;
            ShowSysMsg("Player:" + _runner.GetPlayerUserId(userId) + " joined the room");
        });
        NetworkEventsHandler.PlayerLeft.AddListener(userId =>
            ShowSysMsg("Player:" + _runner.GetPlayerUserId(userId) + " left the room"));
        NetworkEventsHandler.ServerDisconnected.AddListener(msg => ShowSysMsg("Server Disconnected, reason:" + msg));
        NetworkEventsHandler.ConnectFailed.AddListener(msg => ShowSysMsg("Connect failed, reason:" + msg));
        NetworkEventsHandler.SceneLoadDone.AddListener(() => ShowSysMsg("Scene Load Done."));
        NetworkEventsHandler.SceneLoadStart.AddListener(() => ShowSysMsg("Start loading scene."));
        NetworkEventsHandler.HostMigrated.AddListener(() => ShowSysMsg("Host Migrated!"));

        uiDocument = GetComponentInChildren<UIDocument>(true);
    }

    void ShowSysMsg(string msg)
    {
        ShowMessage(MSGTYPE.SYSTEM, msg);
    }

    public void ShowGameMsg(string msg)
    {
        ShowMessage(MSGTYPE.GAME, msg);
    }

    void ShowMessage(MSGTYPE msgType, string msg)
    {
        curMessages.Enqueue(new Message(msgType, msg));
        while (curMessages.Count > 5)
        {
            curMessages.Dequeue();
        }

        string resultMessage = "";

        foreach (var message in curMessages)
        {
            resultMessage += message + "\n";
        }

        if (messageLabel != null)
            messageLabel.text = resultMessage;
    }

    public void SetTimerText(string text)
    {
        timerLabel.text = text;
    }

    public void UpdatePlayerCoins(NetworkDictionary<PlayerRef, int> playerCoins, NetworkRunner runner)
    {
        if (bonusLabel == null)
            return;
        
        string resultStr = "";
        foreach (var playerCoin in playerCoins)
        {
            if (playerCoin.Key == runner.LocalPlayer)
            {
                bonusLabel.text = "Points:" + playerCoin.Value;
            }

            resultStr += runner.GetPlayerUserId(playerCoin.Key) + ":" + playerCoin.Value + " points\n";
        }

        playerListLabel.text = resultStr;
    }
}