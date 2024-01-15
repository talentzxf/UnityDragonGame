using DragonGameNetworkProject;
using Fusion;
using MoreMountains.Feedbacks;
using UnityEngine;

public abstract class Enemy : NetworkBehaviour
{
    [Networked] protected float hp { set; get; } = 100.0f;
    [Networked] private bool isAlive { set; get; } = true;

    [SerializeField] private GameObject indicatorPrefab;
    private GameObject _indicatorGO;

    protected bool IsInvincible() => false;

    public abstract string GetName();

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void DoDamageRpc(float damage)
    {
        if (!IsInvincible())
        {
            hp -= damage;
            if (hp < 0)
            {
                isAlive = false;
            }            
        }
    }
    
    protected AbstractNameTag _nameTag;

    protected NetworkObject _no;
    public override void Spawned()
    {
        base.Spawned();
        _no = GetComponent<NetworkObject>();
    }

    private void Awake()
    {
        _nameTag = GetComponent<AbstractNameTag>();

        _indicatorGO = Instantiate(indicatorPrefab, transform);
        _indicatorGO.SetActive(false);
    }

    private void Start()
    {
        EnemyManager.Instance.RegisterEnemy(this);

        _nameTag.SetTextColor(Color.red);
    }

    public void Lock()
    {
        UIController.Instance.ShowGameMsg($"Locked Enemy:{GetName()}");
        _nameTag.PlayEffect("Locked");

        var textPosition = _nameTag.GetTextPosition();
        _indicatorGO.transform.position = textPosition;
        _indicatorGO.SetActive(true);
        _indicatorGO.transform.Find("Appear")?.GetComponent<MMFeedbacks>().PlayFeedbacks();
    }

    public void Unlock()
    {
        UIController.Instance.ShowGameMsg($"Unlocked Enemy:{GetName()}");
        _nameTag.SetPostFix(null);
        _indicatorGO.SetActive(false);
    }

    public void ShowDistance(float distance)
    {
        _nameTag.SetPostFix($"({distance:F2})");
    }

    protected abstract void DoDie();

    private bool hasDoneDie = false;

    public override void FixedUpdateNetwork()
    {
        if (!isAlive && !hasDoneDie)
        {
            DoDie();
            hasDoneDie = true;
        }
    }
}