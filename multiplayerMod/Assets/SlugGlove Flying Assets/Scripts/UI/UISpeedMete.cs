using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISpeedMete : MonoBehaviour
{
    private PlayerMovement Player;
    public Text Txt;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!Player.Rigid)
            return;

        float Amt = Mathf.Round((Player.Rigid.velocity.magnitude * 0.5f) * Player.ActSpeed);
        Txt.text = Amt.ToString();
    }
}
