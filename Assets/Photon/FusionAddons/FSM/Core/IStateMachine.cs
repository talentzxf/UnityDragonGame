﻿namespace Fusion.Addons.FSM
{
	public interface IStateMachine
	{
		string        Name          { get; }
		IState        ActiveState   { get; }
		IState        PreviousState { get; }

		IState[]      States        { get; }

		void Initialize(StateMachineController controller, NetworkRunner runner);
		void FixedUpdateNetwork();
		void Render();
		void Deinitialize(bool hasState);
		void Reset();

		bool TryActivateState(int stateId, bool allowReset = false);
		bool ForceActivateState(int stateId, bool allowReset = false);
		bool TryDeactivateState(int stateId);
		bool ForceDeactivateState(int stateId);
		bool TryToggleState(int stateId, bool value);
		void ForceToggleState(int stateId, bool value);

		bool? EnableLogging { get; set; }

		// Networking

		int WordCount { get; }
		unsafe void Read(int* ptr);
		unsafe void Write(int* ptr);
		void Interpolate(InterpolationData interpolationData);
	}
}
