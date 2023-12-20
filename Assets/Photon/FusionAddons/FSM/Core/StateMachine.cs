using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Fusion.Addons.FSM
{
	public partial class StateMachine<TState> : IStateMachine where TState : class, IState
	{
		// PUBLIC MEMBERS

		public string        Name             { get; private set; }
		public NetworkRunner Runner           { get; private set; }

		public bool?         EnableLogging    { get; set; }

		public TState[]      States           => _states;

		public int           ActiveStateId    => _activeStateId;
		public int           PreviousStateId  => _previousStateId;
		public int           StateChangeTick  => _stateChangeTick;

		public float         StateTime        => HasStarted == true ? GetStateTime() : 0f;
		public int           StateTicks       => HasStarted == true ? GetStateTicks() : 0;

		public TState        ActiveState      => _states[ActiveStateId];
		public TState        PreviousState    => _states[PreviousStateId];

		public bool          HasStarted       { get { return _bitState.IsBitSet(0); } set { _bitState = _bitState.SetBitNoRef(0, value); } }
		public bool          IsPaused         { get { return _bitState.IsBitSet(1); } set { _bitState = _bitState.SetBitNoRef(1, value); } }


		// PRIVATE MEMBERS

		private readonly TState[] _states;
		private readonly int _stateCount;
		private readonly int _defaultStateId = 0;

		private int _lastRenderStateId = -1;
		private int _lastRenderStateChangeTick = -1;
		private float _interpolationTick;

		private StateMachineController _controller;

		// CONSTRUCTORS

		public StateMachine(string name, params TState[] states)
		{
			Name = name;

			_states = states;
			_stateCount = _states.Length;

			for (int i = 0; i < _states.Length; i++)
			{
				var state = _states[i];

				state.StateId = i;

				if (state is IOwnedState<TState> ownedState)
				{
					ownedState.Machine = this;
				}
			}
		}

		// PUBLIC METHODS

		public bool TryActivateState(int stateId, bool allowReset = false)
		{
			return TryActivateState(stateId, allowReset, false);
		}

		public bool ForceActivateState(int stateId, bool allowReset = false)
		{
			if (stateId == ActiveStateId && allowReset == false)
				return false;

			ChangeState(stateId);
			return true;
		}

		public bool TryDeactivateState(int stateId)
		{
			if (stateId != ActiveStateId)
				return true;

			return TryActivateState(_defaultStateId, false, true);
		}

		public bool ForceDeactivateState(int stateId)
		{
			if (stateId != ActiveStateId)
				return true;

			return ForceActivateState(_defaultStateId);
		}

		public bool TryToggleState(int stateId, bool value)
		{
			bool stateIsActive = stateId == ActiveStateId;

			if (stateIsActive == value)
				return true;

			int targetState = value == true ? stateId : _defaultStateId;
			return TryActivateState(targetState, false, value == false);
		}

		public void ForceToggleState(int stateId, bool value)
		{
			bool stateIsActive = stateId == ActiveStateId;

			if (stateIsActive == value)
				return;

			int targetState = value == true ? stateId : _defaultStateId;
			ForceActivateState(targetState, false);
		}

		public bool HasState(TState state)
		{
			for (int i = 0; i < _stateCount; i++)
			{
				if (_states[i].StateId == state.StateId && _states[i] == state)
					return true;
			}

			return false;
		}

		public TState GetState(int stateId)
		{
			if (stateId < 0 || stateId >= _stateCount)
				return default;

			return _states[stateId];
		}

		public T GetState<T>() where T : TState
		{
			for (int i = 0; i < _stateCount; i++)
			{
				if (_states[i] is T state)
					return state;
			}

			return default;
		}

		public void Reset()
		{
			_activeStateId = _defaultStateId;
			_previousStateId = _defaultStateId;
			_stateChangeTick = 0;
			_bitState = 0;

			_lastRenderStateId = -1;

			if (_hasChildMachines == true)
			{
				for (int i = 0; i < _stateCount; i++)
				{
					var state = _states[i];

					for (int j = 0; j < state.ChildMachines.Length; j++)
					{
						state.ChildMachines[j].Reset();
					}
				}
			}
		}

		// IStateMachine INTERFACE

		void IStateMachine.Initialize(StateMachineController controller, NetworkRunner runner)
		{
			Runner = runner;

			_controller = controller;

			for (int i = 0; i < _stateCount; i++)
			{
				var state = _states[i];

				for (int j = 0; j < state.ChildMachines.Length; j++)
				{
					state.ChildMachines[j].Initialize(controller, runner);
				}

				state.Initialize();
			}
		}

		void IStateMachine.FixedUpdateNetwork()
		{
			if (IsPaused == true)
				return;

			if (HasStarted == false)
			{
				ChangeState(ActiveStateId);
			}

			int updateStateId = ActiveStateId;

			ActiveState.OnFixedUpdate();

			// Active state could be changed in state's fixed update
			// Do not update its child machines in that case
			if (updateStateId == ActiveStateId)
			{
				for (int i = 0; i < ActiveState.ChildMachines.Length; i++)
				{
					ActiveState.ChildMachines[i].FixedUpdateNetwork();
				}
			}
		}

		void IStateMachine.Render()
		{
			if (HasStarted == false || IsPaused == true)
				return;

			if (_lastRenderStateId != ActiveStateId || _lastRenderStateChangeTick != _stateChangeTick)
			{
				LogRenderStateChange();

				if (_lastRenderStateId >= 0)
				{
					_states[_lastRenderStateId].OnExitStateRender();
				}

				ActiveState.OnEnterStateRender();

				_lastRenderStateId = ActiveStateId;
				_lastRenderStateChangeTick = _stateChangeTick;
			}

			int updateStateId = ActiveStateId;

			ActiveState.OnRender();

			if (updateStateId != ActiveStateId)
			{
				Assert.Fail($"Active state should not be changed during Render, Machine: {Name}");
			}

			for (int i = 0; i < ActiveState.ChildMachines.Length; i++)
			{
				ActiveState.ChildMachines[i].Render();
			}
		}

		void IStateMachine.Deinitialize(bool hasState)
		{
			for (int i = 0; i < _stateCount; i++)
			{
				var state = _states[i];

				state.Deinitialize(hasState);

				for (int j = 0; j < state.ChildMachines.Length; j++)
				{
					state.ChildMachines[j].Deinitialize(hasState);
				}
			}

			Runner = null;
		}

		// PRIVATE METHODS

		private bool TryActivateState(int stateId, bool allowReset, bool isExplicitDeactivation)
		{
			if (stateId == ActiveStateId && allowReset == false)
				return false;

			var nextState = _states[stateId];

			if (ActiveState.CanExitState(nextState, isExplicitDeactivation) == false)
				return false;

			if (nextState.CanEnterState() == false)
				return false;

			ChangeState(stateId);
			return true;
		}

		private void ChangeState(int stateId)
		{
			if (stateId >= _stateCount)
			{
				throw new InvalidOperationException($"State with ID {stateId} not present in the state machine {Name}");
			}

			Assert.Check(Runner.Stage != default, "State changes are not allowed from Render calls");

			_previousStateId = ActiveStateId;
			_activeStateId = stateId;

			LogStateChange();

			if (HasStarted == true)
			{
				PreviousState.OnExitState();
			}

			_stateChangeTick = Runner.Tick;

			ActiveState.OnEnterState();
			HasStarted = true;
		}

		private int GetStateTicks()
		{
			int currentTick = Runner.Stage == default && _interpolationTick != 0f ? (int)_interpolationTick : Runner.Tick;
			return currentTick - StateChangeTick;
		}

		private float GetStateTime()
		{
			if (Runner.Stage != default || _interpolationTick == 0f)
				return (Runner.Tick - StateChangeTick) * Runner.DeltaTime;

			return (_interpolationTick - StateChangeTick) * Runner.DeltaTime;
		}

		// LOGGING

		[Conditional("DEBUG")]
		private void LogStateChange()
		{
			if (EnableLogging.HasValue == false && _controller.EnableLogging == false)
				return; // Global controller logging is disabled

			if (EnableLogging.HasValue == true && EnableLogging.Value == false)
				return; // Logging is specifically disabled for this machine

			if (HasStarted == true)
			{
				Debug.Log($"{_controller.gameObject.name} - <color=#F04C4C>State Machine <b>{Name}</b>: Change State to <b>{ActiveState.Name}</b></color> - Previous: {PreviousState.Name}, Tick: {Runner.Tick.Raw}", _controller);
			}
			else
			{
				Debug.Log($"{_controller.gameObject.name} - <color=#F04C4C>State Machine <b>{Name}</b>: Change State to <b>{ActiveState.Name}</b></color> - Tick: {Runner.Tick.Raw}", _controller);
			}
		}

		[Conditional("DEBUG")]
		private void LogRenderStateChange()
		{
			if (EnableLogging.HasValue == false && _controller.EnableLogging == false)
				return; // Global controller logging is disabled

			if (EnableLogging.HasValue == true && EnableLogging.Value == false)
				return; // Logging is specifically disabled for this machine

			if (_lastRenderStateId >= 0)
			{
				Debug.Log($"{_controller.gameObject.name} - <color=#467DE7>State Machine <b>{Name}</b>: Change RENDER State to <b>{ActiveState.Name}</b></color> - Previous: {_states[_lastRenderStateId].Name}, StateChangeTick: {_stateChangeTick}, RenderFrame: {Time.frameCount}", _controller);
			}
			else
			{
				Debug.Log($"{_controller.gameObject.name} - <color=#467DE7>State Machine <b>{Name}</b>: Change RENDER State to <b>{ActiveState.Name}</b></color> - StateChangeTick: {_stateChangeTick}, RenderFrame: {Time.frameCount}", _controller);
			}
		}
	}
}
