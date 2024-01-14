using System.ComponentModel.Design.Serialization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

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
    [SerializeField] private Texture avatarRenderTexture;

    public UnityEvent<Color> onBodyColorPicked;
    public UnityEvent<Color> onHairColorPicked;

    private static PrepareUI _instance;

    public static PrepareUI Instance => _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        
        uiDoc = GetComponent<UIDocument>();

        Image img1P = uiDoc.rootVisualElement.Q<Image>("AvatarImage_1P");
        img1P.image = avatarRenderTexture;

        var colorPickerContainer = uiDoc.rootVisualElement.Q<VisualElement>("ColorPickers");
        colorPickerContainer.Add(new ColorPicker("Hair Color", onHairColorPicked));
        colorPickerContainer.Add(new ColorPicker("Body Color", onBodyColorPicked));
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
