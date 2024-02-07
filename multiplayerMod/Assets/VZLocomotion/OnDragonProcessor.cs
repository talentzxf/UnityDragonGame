using UnityEngine;

public class OnDragonProcessor : OnDragonLocomotionProcessor
{
    private CharacterController _character;

    private int _climbdown = Animator.StringToHash("climbDown");

    public override void Setup(CharacterLocomotion locomotion)
    {
        base.Setup(locomotion);
        _character = _go.GetComponent<CharacterController>();
    }

    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("Clicked M, climbing down");
            _character.enabled = true;
            _animator.SetTrigger(_climbdown);
            _transform.SetParent(null);

            _dragonGO.GetComponent<CharacterLocomotion>().SetProcessor<DragonIdleProcessor>();
        }
    }
}