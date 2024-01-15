using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using DragonGameNetworkProject;
using DragonGameNetworkProject.DragonAvatarMovements;
using ExitGames.Client.Photon.StructWrapping;
using Fusion;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.UIElements;
using ColorUtility = UnityEngine.ColorUtility;

class ColorPicker : VisualElement
{
    private Color[] preDefinedColors =
    {
        Color.black, Color.blue, Color.cyan, Color.gray, Color.green, Color.magenta, Color.red, Color.white,
        Color.yellow
    };
    
    public ColorPicker(string label, UnityEvent<Color> onColorClicked)
    {
        style.flexDirection = FlexDirection.Row;
        style.flexWrap = Wrap.Wrap;
        var labelEle = new Label(label)
        {
            style =
            {
                width = new Length(100.0f, LengthUnit.Percent)
            }
        };
        Add(labelEle);

        foreach (var color in preDefinedColors)
        {
            var button = new Button
            {
                style =
                {
                    width = new Length(30.0f, LengthUnit.Percent),
                    backgroundColor = color
                }
            };
            button.clicked += ()=>
            {
                onColorClicked?.Invoke(color);
            };
            
            Add(button);
        }
    }
}

public class PrepareUI : MonoBehaviour
{
    private UIDocument uiDoc;

    public UnityEvent<Color> onBodyColorPicked;
    public UnityEvent<Color> onHairColorPicked;

    private Button readyOrStartBtn;

    private static PrepareUI _instance;
    public static PrepareUI Instance => _instance;

    private string PlayerUIPath = "Assets/DragonGameNetworkProject/UI/PlayerSelector.uxml";

    public void SetupAvatarUI(NetworkRunner runner, PlayerRef playerRef, Texture texture)
    {
        _runner = runner;
        uiDoc.enabled = true;
        enabled = true;
        
        var leftBar = uiDoc.rootVisualElement.Q<VisualElement>("Left");

        VisualElement playerEle = new VisualElement();
        playerEle.style.borderBottomLeftRadius = playerEle.style.borderBottomRightRadius =
            playerEle.style.borderTopLeftRadius =
                playerEle.style.borderTopRightRadius = new Length(10, LengthUnit.Pixel);
        ColorUtility.TryParseHtmlString("#FFC200", out Color bgColor);
        playerEle.style.backgroundColor = bgColor;
        playerEle.style.marginTop = playerEle.style.marginBottom =
            playerEle.style.marginLeft = playerEle.style.marginRight = new Length(50, LengthUnit.Pixel);
        playerEle.style.width = new Length(33, LengthUnit.Percent);
        playerEle.style.height = new Length(40, LengthUnit.Percent);

        Image avatarImg = new Image();
        avatarImg.image = texture;
        
        Label nameLabel = new Label();
        nameLabel.text = "Player:" + _runner.GetPlayerUserId(playerRef);
        playerEle.Add(avatarImg);
        playerEle.Add(nameLabel);
        leftBar.Add(playerEle);
    }

    private HashSet<DragonAvatarController> _controllers = new();

    public void RegisterDragonAvatarController(DragonAvatarController controller)
    {
        _controllers.Add(controller);
    }

    private void Update()
    {
        if (_controllers == null || _runner == null)
            return;

        if (_runner.IsSharedModeMasterClient || _runner.IsSinglePlayer)
        {
            bool allReady = true;
        
            foreach (var controller in _controllers)
            {
                if (!controller.isReady && !controller.HasInputAuthority)
                {
                    allReady = false;
                }
            }

            if (allReady)
            {
                readyOrStartBtn.text = "Start";
                readyOrStartBtn.SetEnabled(true);
            }
            else
            {
                readyOrStartBtn.text = "Waiting";
                readyOrStartBtn.SetEnabled(false);
            }
        }
        else
        {
            foreach (var controller in _controllers)
            {
                if (controller.HasInputAuthority)
                {
                    if (controller.isReady)
                    {
                        readyOrStartBtn.text = "Not Ready";
                    }
                    else
                    {
                        readyOrStartBtn.text = "Ready";
                    }
                }
            }
            readyOrStartBtn.SetEnabled(true);
        }
    }

    private NetworkRunner _runner;
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        uiDoc = GetComponent<UIDocument>();
        
        var colorPickerContainer = uiDoc.rootVisualElement.Q<VisualElement>("ColorPickers");
        colorPickerContainer.Add(new ColorPicker("Hair Color", onHairColorPicked));
        colorPickerContainer.Add(new ColorPicker("Body Color", onBodyColorPicked));

        readyOrStartBtn = uiDoc.rootVisualElement.Q<Button>("StartOrReady");
        
        readyOrStartBtn.clicked += () =>
        {
            foreach (var controller in _controllers)
            {
                if (controller.HasInputAuthority)
                {
                    controller.isReady = !controller.isReady;
                }
            }
        };
        
        NetworkEventsHandler.ServerConnected.AddListener(runner =>
        {
            ActivateUiDocument();
            string roomName = runner.SessionInfo.Name + "@" + runner.SessionInfo.Region;
            uiDoc.rootVisualElement.Q<Label>("RoomName").text = "Room:" + roomName;
            
            uiDoc.rootVisualElement.Q<Label>("GameMode").text = "Collect Coins";
            uiDoc.rootVisualElement.Q<Label>("GameTime").text = GameTimer.Instance.TotalTime + "s";
        
            _runner = runner;
        });
    }

    public void ActivateUiDocument()
    {
        uiDoc.gameObject.SetActive(true);
    }
    
    // Texture2D GenerateTexture()
    // {
    //     int width = 256;
    //     int height = 256;
    //
    //     Texture2D texture = new Texture2D(width, height);
    //
    //     for (int y = 0; y < height; y++)
    //     {
    //         for (int x = 0; x < width; x++)
    //         {
    //             // float r = Mathf.Lerp(0.0f, 1.0f, Mathf.Sin(Mathf.PI * x / (float)width));
    //             // float g = Mathf.Lerp(0.0f, 1.0f, Mathf.Sin(Mathf.PI * y / (float)height));
    //             float r = Mathf.Lerp(0.0f, 1.0f, Mathf.Sin(Mathf.PI * x / (float)width));
    //             float g = Mathf.Lerp(0.0f, 1.0f, Mathf.Sin(Mathf.PI * y / (float)height));
    //             float b = Mathf.Lerp(0.0f, 1.0f, Mathf.Sqrt(x * x * x + y * y * y) / Mathf.Sqrt(width * width * width + height * height * height));
    //             
    //             Color color = new Color(r, g, b);
    //             texture.SetPixel(x, y, color);
    //         }
    //     }
    //
    //     texture.Apply();
    //
    //     return texture;
    // }
}
