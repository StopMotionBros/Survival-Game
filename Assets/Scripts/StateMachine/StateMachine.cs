using UnityEngine;

public class StateMachine : MonoBehaviour
{
	public State CurrentState => _currentState;
	State _currentState;

	public void SetState(State state)
	{
		if (_currentState != null) _currentState.Exit();
		_currentState = state;
		_currentState.Enter();
	}
}