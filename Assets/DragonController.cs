using System;
using UnityEngine;

public class DragonController : MonoBehaviour
{
    private bool mounted = false;

    private Animator _animator;
    
    int takeOff = Animator.StringToHash("TakeOff");
    
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
            _animator.SetTrigger(takeOff);
        }
    }
}
