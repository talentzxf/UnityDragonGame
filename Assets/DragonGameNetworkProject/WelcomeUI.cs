using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Fusion;
using UnityEditor;
using UnityEngine;
using WebSocketSharp;
using Behaviour = Fusion.Behaviour;
using Object = UnityEngine.Object;

[RequireComponent(typeof(NetworkBootStrap))]
[AddComponentMenu("DragonSkyDanceGUI")]
[ScriptHelp(BackColor = ScriptHeaderBackColor.Steel)]
public class WelcomeUI : Behaviour
{
    [InlineHelp] public GUISkin BaseSkin;
    private NetworkBootStrap _networkDebugStart;
    string _clientCount;

    Dictionary<NetworkBootStrap.Stage, string> _nicifiedStageNames;
#if UNITY_EDITOR

    protected virtual void Reset()
    {
        _networkDebugStart = EnsureNetworkDebugStartExists();
        _clientCount = _networkDebugStart.AutoClients.ToString();
        BaseSkin = GetAsset<GUISkin>("e59b35dfeb4b6f54e9b2791b2a40a510");
    }

    public static T GetAsset<T>(string Guid) where T : Object
    {
        var path = AssetDatabase.GUIDToAssetPath(Guid);
        if (string.IsNullOrEmpty(path))
        {
            return null;
        }
        else
        {
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }
    }

#endif

    protected int GetClientCount()
    {
        try
        {
            return Convert.ToInt32(_clientCount);
        }
        catch
        {
            return 0;
        }
    }

    protected void ValidateClientCount()
    {
        if (_clientCount == null)
        {
            _clientCount = "1";
        }
        else
        {
            _clientCount = Regex.Replace(_clientCount, "[^0-9]", "");
        }
    }

    protected virtual void Awake()
    {
        _nicifiedStageNames = ConvertEnumToNicifiedNameLookup<NetworkBootStrap.Stage>("Network Status: ");
        _networkDebugStart = EnsureNetworkDebugStartExists();
        _clientCount = _networkDebugStart.AutoClients.ToString();
        ValidateClientCount();
    }

    protected NetworkBootStrap EnsureNetworkDebugStartExists()
    {
        if (_networkDebugStart)
        {
            if (_networkDebugStart.gameObject == gameObject)
                return _networkDebugStart;
        }

        if (TryGetBehaviour<NetworkBootStrap>(out var found))
        {
            _networkDebugStart = found;
            return found;
        }

        _networkDebugStart = AddBehaviour<NetworkBootStrap>();
        return _networkDebugStart;
    }

    // TODO Move to a utility
    public static Dictionary<T, string> ConvertEnumToNicifiedNameLookup<T>(string prefix = null,
        Dictionary<T, string> nonalloc = null) where T : Enum
    {
        StringBuilder sb = new StringBuilder();

        if (nonalloc == null)
        {
            nonalloc = new Dictionary<T, string>();
        }
        else
        {
            nonalloc.Clear();
        }

        var names = Enum.GetNames(typeof(T));
        var values = Enum.GetValues(typeof(T));
        for (int i = 0, cnt = names.Length; i < cnt; ++i)
        {
            sb.Clear();
            if (prefix != null)
            {
                sb.Append(prefix);
            }

            var name = names[i];
            for (int n = 0; n < name.Length; n++)
            {
                // If this character is a capital and it is not the first character add a space.
                // This is because we don't want a space before the word has even begun.
                if (char.IsUpper(name[n]) == true && n != 0)
                {
                    sb.Append(" ");
                }

                // Add the character to our new string
                sb.Append(name[n]);
            }

            nonalloc.Add((T) values.GetValue(i), sb.ToString());
        }

        return nonalloc;
    }

    private bool showAlert = false;
    private string alertString = "";
    
    private void ShowAlertWindow(int windowID)
    {
        GUILayout.Label(alertString);
        
        GUILayout.Space(20);

        if (GUILayout.Button("OK"))
        {
            // 点击OK按钮后关闭弹窗
            showAlert = false;
        }
    }
    
    protected virtual void OnGUI()
    {
        var nds = EnsureNetworkDebugStartExists();
        if (nds.StartMode != NetworkBootStrap.StartModes.UserInterface)
        {
            return;
        }

        var currentstage = nds.CurrentStage;
        if (nds.AutoHideGUI && currentstage == NetworkBootStrap.Stage.AllConnected)
        {
            return;
        }

        var holdskin = GUI.skin;

        GUI.skin = FusionScalableIMGUI.GetScaledSkin(BaseSkin, out var height, out var width, out var padding,
            out var margin, out var leftBoxMargin);

        if (showAlert)
        {
            GUI.ModalWindow(0, new Rect(Screen.width / 2 - 150, Screen.height / 2 - 75, 600, 300), ShowAlertWindow, "Alert");
            return;
        }

        GUILayout.BeginArea(new Rect(leftBoxMargin, margin, width, Screen.height));
        {
            GUILayout.BeginVertical(GUI.skin.window);
            {
                GUILayout.BeginHorizontal(GUILayout.Height(height));
                {
                    var stagename = _nicifiedStageNames.TryGetValue(nds.CurrentStage, out var stage)
                        ? stage
                        : "Unrecognized Stage";
                    GUILayout.Label(stagename,
                        new GUIStyle(GUI.skin.label)
                            {fontSize = (int) (GUI.skin.label.fontSize * .8f), alignment = TextAnchor.UpperLeft});

                    // Add button to hide Shutdown option after all connect, which just enables AutoHide - so that interface will reappear after a disconnect.
                    if (nds.AutoHideGUI == false && nds.CurrentStage == NetworkBootStrap.Stage.AllConnected)
                    {
                        if (GUILayout.Button("X", GUILayout.ExpandHeight(true), GUILayout.Width(height)))
                        {
                            nds.AutoHideGUI = true;
                        }
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
            
            GUILayout.BeginVertical(GUI.skin.window);
            {
                if (currentstage == NetworkBootStrap.Stage.Disconnected)
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUIStyle customStyle = new GUIStyle(GUI.skin.label);
                        customStyle.fontSize = 32;
                        customStyle.hover = customStyle.normal;
                        // customStyle.border = new RectOffset(0, 0, 0, 0);
                        customStyle.alignment = TextAnchor.MiddleCenter;
                        customStyle.margin = new RectOffset(0, 0, 30, 30);
                        
                        GUILayout.Label("小龙龙的奇幻之旅\nLittle Dragon's Fantastic Journey", customStyle);
                    }
                    GUILayout.EndHorizontal();
                    
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("Room:", GUILayout.Height(height), GUILayout.Width(width * .33f));
                        nds.DefaultRoomName = GUILayout.TextField(nds.DefaultRoomName, 25, GUILayout.Height(height));

                    }
                    GUILayout.EndHorizontal();
                    
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("NickName:", GUILayout.Height(height), GUILayout.Width(width * .33f));
                        nds.NickName = GUILayout.TextField(nds.NickName, 25, GUILayout.Height(height));
                    }
                    GUILayout.EndHorizontal();
                        
                    if (GUILayout.Button("Join or Create Room", GUILayout.Height(height)))
                    {
                        if (nds.DefaultRoomName.IsNullOrEmpty() || nds.NickName.IsNullOrEmpty())
                        {
                            alertString = "RoomName/NickName is empty?";
                            showAlert = true;
                        }
                        else
                        {
                            nds.StartSharedClient();
                        }
                    }
                    
                    // if (GUILayout.Button("Start Single Player", GUILayout.Height(height)))
                    // {
                    //     nds.StartSinglePlayer();
                    // }
                }
                else
                {
                    if (GUILayout.Button("Shutdown", GUILayout.Height(height)))
                    {
                        _networkDebugStart.ShutdownAll();
                    }
                }

                GUILayout.EndVertical();
            }
        }
        GUILayout.EndArea();

        GUI.skin = holdskin;
    }
}