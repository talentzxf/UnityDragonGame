using System;
using UnityEngine;

public class DragonController : MonoBehaviour
{
    private bool mounted = false;

    private Animator _animator;
    
    int IdleSimple = Animator.StringToHash("IdleSimple");
    int IdleAgressive = Animator.StringToHash("IdleAgressive");
    int IdleRestless = Animator.StringToHash("IdleRestless");
    int Walk = Animator.StringToHash("Walk");
    int BattleStance = Animator.StringToHash("BattleStance");
    int Bite = Animator.StringToHash("Bite");
    int Drakaris = Animator.StringToHash("Drakaris");
    int FlyingFWD = Animator.StringToHash("FlyingFWD");
    int FlyingAttack = Animator.StringToHash("FlyingAttack");
    int Hover = Animator.StringToHash("Hover");
    int Lands = Animator.StringToHash("Lands");
    int TakeOff = Animator.StringToHash("TakeOff");
    int Die = Animator.StringToHash("Die");
    
    void Start()
    {
        _animator = GetComponent<Animator>();
    }
    
    public void SetMounted(bool isMounted)
    {
        mounted = isMounted;
    }
    
    void Update()
    {
        if (!mounted)
            return;
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _animator.SetBool(IdleSimple, true);
            _animator.SetBool(IdleAgressive, false);
            _animator.SetBool(IdleRestless, false);
            _animator.SetBool(Walk, false);
            _animator.SetBool(BattleStance, false);
            _animator.SetBool(Bite, false);
            _animator.SetBool(Drakaris, false);
            _animator.SetBool(FlyingFWD, false);
            _animator.SetBool(FlyingAttack, false);
            _animator.SetBool(Hover, false);
            _animator.SetBool(Lands, false);
            _animator.SetBool(TakeOff, false);
            _animator.SetBool(Die, false);
        }
    }
}
