using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    private UIDocument uiDocument;
    private void Awake()
    {
        NetworkEventsHandler.LocalPlayerJoined += (sender, userId) =>
        {
            uiDocument.gameObject.SetActive(true);

            string localPlayerId = userId;

            uiDocument.rootVisualElement.Q<Label>("LocalUserID").text = "UserID:" + localPlayerId;
        };

        NetworkEventsHandler.ServerConnected += (sender, runner) =>
        {
            uiDocument.gameObject.SetActive(true);

            string roomName = runner.SessionInfo.Name + "@" + runner.SessionInfo.Region;
            uiDocument.rootVisualElement.Q<Label>("RoomName").text = "Room:" + roomName;
        };
        
        uiDocument = GetComponentInChildren<UIDocument>(true);
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
