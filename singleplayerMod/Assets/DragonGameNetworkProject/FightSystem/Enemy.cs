using DragonGameNetworkProject;
using DragonGameNetworkProject.FightSystem;
using MoreMountains.Feedbacks;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
     protected float hp { set; get; } = 100.0f;
     private bool isAlive { set; get; } = true;

    [SerializeField] private GameObject indicatorPrefab;
    private GameObject _indicatorGO;

    protected bool IsInvincible() => false;

    public abstract string GetName();

    //[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void DoDamageRpc( float damage)
    {
        if (!IsInvincible())
        {
            hp -= damage;
            //DoDamage(doneBy);
            if (hp < 0)
            {
                isAlive = false;
            }
        }
    }

    // protected virtual void DoDamage(PlayerRef doneBy)
    // {
    //     
    // }

    protected AbstractNameTag _nameTag;

   protected GameObject _no;

    public  void Spawned()
    {
        _no = GetComponent<GameObject>();
        if (_no == null)
            _no = GetComponentInParent<GameObject>();
    }

    private void Start()
    {
        if (this is PlayerEnemy ) // Don't lock myself.
        {
            return;
        }

        Spawned();
        
        _nameTag = GetComponent<AbstractNameTag>();
        if (_nameTag == null)
            _nameTag = GetComponentInParent<AbstractNameTag>();

        _indicatorGO = Instantiate(indicatorPrefab, transform);
        _indicatorGO.SetActive(false);
        
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

    private void Update()
    {
        // if (Runner == null || Runner.State != NetworkRunner.States.Running)
        //     return;

        if (!isAlive && !hasDoneDie)
        {
            DoDie();
            hasDoneDie = true;
        }
    }
}