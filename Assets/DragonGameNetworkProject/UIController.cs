using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController : NetworkBehaviour
{
    private UIDocument uiDocument;
    private void Awake()
    {
        GlobalEvents.Instance.LocalPlayerJoined += (sender, e) =>
        {
            string localPlayerId = Runner.GetPlayerUserId(e);

            uiDocument.rootVisualElement.Q<Label>("LocalUserID").text = "UserID:" + localPlayerId;
        };

        uiDocument = GetComponentInChildren<UIDocument>();
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
