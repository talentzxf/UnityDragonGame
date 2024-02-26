using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class vButtonOnOFF : MonoBehaviour
{
    bool isInUse;
    public string Name;

    public void SetDownState()
    {
        if (!isInUse)
        {
            CrossPlatformInputManager.SetButtonDown(Name);
            isInUse = true;
        }
        else
        {
            isInUse = false;
            CrossPlatformInputManager.SetButtonUp(Name);
        }
    }

    public void SetUpState()
    {
        CrossPlatformInputManager.SetButtonUp(Name);
    }
}
