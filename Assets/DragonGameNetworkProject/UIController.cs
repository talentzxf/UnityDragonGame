using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    private UIDocument uiDocument;

    private Label messageLabel;

    private Queue<string> curMessages = new Queue<string>();

    private void ActivateUiDocument()
    {
        uiDocument.gameObject.SetActive(true);
        messageLabel = uiDocument.rootVisualElement.Q<Label>("Messages");
        messageLabel.text = "";
    }
    
    private void Awake()
    {
        NetworkEventsHandler.LocalPlayerJoined += (sender, userId) =>
        {
            ActivateUiDocument();
            string localPlayerId = userId;

            uiDocument.rootVisualElement.Q<Label>("LocalUserID").text = "UserID:" + localPlayerId;
        };

        NetworkEventsHandler.ServerConnected += (sender, runner) =>
        {
            ActivateUiDocument();
            
            uiDocument.gameObject.SetActive(true);

            string roomName = runner.SessionInfo.Name + "@" + runner.SessionInfo.Region;
            uiDocument.rootVisualElement.Q<Label>("RoomName").text = "Room:" + roomName;
            
            ShowMessage("Connected to server.");
        };

        NetworkEventsHandler.PlayerJoined += (sender, userId) =>
        {
            ShowMessage("Player:" + userId + " joined the room");
        };
        
        NetworkEventsHandler.PlayerLeft += (sender, userId) =>
        {
            ShowMessage("Player:" + userId + " joined the room");
        };
        
        uiDocument = GetComponentInChildren<UIDocument>(true);
    }

    void ShowMessage(string msg)
    {
        curMessages.Enqueue(msg);
        while (curMessages.Count > 5)
        {
            curMessages.Dequeue();
        }

        string resultMessage = "";

        foreach (var message in curMessages)
        {
            resultMessage += message + "\n";
        }

        if(messageLabel != null)
            messageLabel.text = resultMessage;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
