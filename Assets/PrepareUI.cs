using UnityEngine;
using UnityEngine.UIElements;

public class PrepareUI : MonoBehaviour
{
    private UIDocument uiDoc;
    [SerializeField] private Texture avatarRenderTexture;

    private Color[] preDefinedColors =
    {
        Color.black, Color.blue, Color.cyan, Color.gray, Color.green, Color.magenta, Color.red, Color.white,
        Color.yellow
    };
    
    private void Awake()
    {
        uiDoc = GetComponent<UIDocument>();

        Image img1P = uiDoc.rootVisualElement.Q<Image>("AvatarImage_1P");
        img1P.image = avatarRenderTexture;

        var ColorPicker = uiDoc.rootVisualElement.Q<VisualElement>("ColorPicker");
        // Generate a 2d texture.
        foreach (var color in preDefinedColors)
        {
            var button = new Button();
            button.style.backgroundColor = color;
            ColorPicker.Add(button);            
        }
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
