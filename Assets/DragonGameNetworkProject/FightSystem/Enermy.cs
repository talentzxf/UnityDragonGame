using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class Enermy : MonoBehaviour
{
    [SerializeField] private float hp = 100.0f;

    public abstract string GetName();

    public void DoDamage(float power)
    {
        hp -= power;
        if (hp < 0)
        {
            DoDie();
        }
    }

    protected abstract void DoDie();

    private void Update()
    {
        // Show the nametag on user's screen.
    }
}
