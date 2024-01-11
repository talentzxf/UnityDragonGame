using DragonGameNetworkProject;
using MoreMountains.Feedbacks;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] private float hp = 100.0f;
    [SerializeField] private GameObject indicatorPrefab;
    private GameObject _indicatorGO;

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

    private void Update()
    {
        // Show the nametag on user's screen.
    }
}
