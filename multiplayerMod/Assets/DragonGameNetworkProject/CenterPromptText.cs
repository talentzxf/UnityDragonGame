using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CenterPromptText : MonoBehaviour
{
    private TextMeshProUGUI text;

    static private CenterPromptText _instance;

    static public CenterPromptText Instance => _instance;
    
    private void Awake()
    {
        if (_instance != null && _instance.gameObject != gameObject)
        {
            Destroy(_instance);
            return;
        }

        _instance = this;
        
        text = GetComponent<TextMeshProUGUI>();

        text.enabled = false;
    }

    public void ShowCenterPrompt(string msg, float ttlSeconds = 3.0f)
    {
        text.text = msg;
        text.enabled = true;
        
        Invoke("HideText", ttlSeconds);
    }

    private void HideText()
    {
        text.enabled = false;
    }
}
