using UnityEngine;
using UnityEngine.UIElements;

public class Utility
{
    public static Image CreateImageInAvatarView(Texture texture)
    {
        var img = new Image();
        img.image = texture;
        img.style.scale = new Vector2(1.5f, 1.0f);

        img.style.width = 30f;
        img.style.height = 60f;

        img.style.position = Position.Absolute;
        img.style.left = 20f;
        img.style.top = 20f;

        return img;
    }
    
    // public static VisualElement SetupAvatarUI(NetworkRunner runner, VisualElement containerEle, PlayerRef playerRef, Texture texture)
    // {
    //     VisualElement playerEle = new VisualElement();
    //     playerEle.style.borderBottomLeftRadius = playerEle.style.borderBottomRightRadius =
    //         playerEle.style.borderTopLeftRadius =
    //             playerEle.style.borderTopRightRadius = new Length(10, LengthUnit.Pixel);
    //     ColorUtility.TryParseHtmlString("#f7e0ac", out Color bgColor);
    //     playerEle.style.backgroundColor = bgColor;
    //     playerEle.style.marginTop = playerEle.style.marginBottom =
    //         playerEle.style.marginLeft = playerEle.style.marginRight = new Length(20, LengthUnit.Pixel);
    //     playerEle.style.width = new Length(33, LengthUnit.Percent);
    //     playerEle.style.height = new Length(40, LengthUnit.Percent);
    //
    //     Image avatarImg = new Image();
    //     avatarImg.image = texture;
    //     
    //     Label nameLabel = new Label();
    //     nameLabel.text = "Player:" + runner.GetPlayerUserId(playerRef);
    //     playerEle.Add(avatarImg);
    //     playerEle.Add(nameLabel);
    //     containerEle.Add(playerEle);
    //     
    //     return playerEle;
    // }
    
    public static void SetLayerRecursively(GameObject obj, int layerMask)
    {
        obj.layer = layerMask;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layerMask);
        }
    }
    
    public static bool IsVisibleOnScreen(Camera cam, Vector3 targetPoint, out Vector3 screenPoint)
    {
        screenPoint = cam.WorldToScreenPoint(targetPoint);

        Vector3 cameraRay = targetPoint - cam.transform.position;

        if (Vector3.Dot(cam.transform.forward, cameraRay) < 0) // The point is in the back of the camera.
        {
            return false;
        }

        return (screenPoint.x >= 0 && screenPoint.x <= Screen.width && screenPoint.y >= 0 &&
                screenPoint.y <= Screen.height);
    }
    
    public static Transform RecursiveFind(Transform parent, string childName)
    {
        Transform result = null;

        foreach (Transform child in parent)
        {
            if (child.name == childName)
            {
                // 找到了目标子对象
                result = child;
                break;
            }
            else
            {
                // 递归调用，在当前子对象的子对象中继续查找
                result = RecursiveFind(child, childName);
                if (result != null)
                {
                    break;
                }
            }
        }

        return result;
    }
}