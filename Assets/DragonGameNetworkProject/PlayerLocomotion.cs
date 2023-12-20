using System.Collections.Generic;
using Fusion;
using Fusion.Addons.FSM;
using UnityEngine;

[RequireComponent(typeof(StateMachineController))]
public class PlayerLocomotion : NetworkBehaviour, IStateMachineOwner
{
    [SerializeField] private PlayerGroundMovement _playerGroundMovement;

    private StateMachine<StateBehaviour> _playerLocomotion;
    public void CollectStateMachines(List<IStateMachine> stateMachines)
    {
        _playerGroundMovement = GetComponent<PlayerGroundMovement>();
        _playerLocomotion = new StateMachine<StateBehaviour>("PlayerLocomotion", _playerGroundMovement);
        
        stateMachines.Add(_playerLocomotion);
    }
}
