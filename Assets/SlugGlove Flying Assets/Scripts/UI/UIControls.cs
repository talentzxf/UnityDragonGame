using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIControls : MonoBehaviour
{

    int indexAmt;
    public UIControlSet[] UISets;

    private void Start()
    {
        indexAmt = 0;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(UISets[indexAmt].Input))
        {
            UISets[indexAmt].UIHandle.SetActive(false);
            indexAmt += 1;

            if(UISets.Length > indexAmt)
            {
                UISets[indexAmt].UIHandle.SetActive(true);
            }
            else
            {
                Destroy(GetComponent<UIControls>());
            }

        }
    }
}

[System.Serializable]
public class UIControlSet
{
    public GameObject UIHandle;
    public string Input;
}
