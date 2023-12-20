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

    public StateMachine<StateBehaviour> StateMachine
    {
        get { return _playerLocomotion; }
    }

    public void CollectStateMachines(List<IStateMachine> stateMachines)
    {
        _playerGroundMovement = GetComponent<PlayerGroundMovement>();
        _playerClimbDragon = GetComponent<PlayerClimbDragon>();
    
        _playerLocomotion = new StateMachine<StateBehaviour>("PlayerLocomotion", _playerGroundMovement, _playerClimbDragon);

        stateMachines.Add(_playerLocomotion);
    }
}