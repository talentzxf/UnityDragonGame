using System.Collections.Generic;
using Fusion;
using Fusion.Addons.FSM;
using UnityEngine;

[RequireComponent(typeof(StateMachineController))]
public class PlayerLocomotion : NetworkBehaviour, IStateMachineOwner
{
    [SerializeField] private PlayerGroundMovement _playerGroundMovement;
    [SerializeField] private PlayerClimbDragon _playerClimbDragon;

    private StateMachine<StateBehaviour> _playerLocomotion;

    private IState _nextState = null;

    public StateMachine<StateBehaviour> StateMachine
    {
        get
        {
            return _playerLocomotion;
        }
    }

    public void SetNextState<T>() where T:IState
    {
        _nextState = _playerLocomotion.GetState<T>();
    }

    public override void FixedUpdateNetwork()
    {
        if (_nextState != null)
        {
            _playerLocomotion.TryActivateState(_nextState.StateId);
        }
    }

    public void CollectStateMachines(List<IStateMachine> stateMachines)
    {
        _playerLocomotion = new StateMachine<StateBehaviour>("PlayerLocomotion", _playerGroundMovement, _playerClimbDragon);

        stateMachines.Add(_playerLocomotion);
    }
}