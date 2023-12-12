using UnityEngine;

public class Utility
{
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