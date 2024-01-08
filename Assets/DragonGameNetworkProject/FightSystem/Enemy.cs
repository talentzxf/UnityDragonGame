using System;
using System.Collections;
using System.Collections.Generic;
using DragonGameNetworkProject;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class Enemy : MonoBehaviour
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

    private AbstractNameTag _nameTag;
    private void Awake()
    {
        _nameTag = GetComponent<AbstractNameTag>();
    }

    private void Start()
    {
        EnemyManager.Instance.RegisterEnemy(this);
    }

    public void Lock()
    {
        UIController.Instance.ShowGameMsg($"Locked Enemy:{GetName()}");
    }

    public void Unlock()
    {
        UIController.Instance.ShowGameMsg($"Unlocked Enemy:{GetName()}");
        _nameTag.SetPostFix(null);
    }

    public void ShowDistance(float distance)
    {
        _nameTag.SetPostFix($"({distance:F2})");
    }

    private void Update()
    {
        // Show the nametag on user's screen.
    }
}
