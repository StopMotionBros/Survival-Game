public abstract class State
{
	protected abstract bool _rootState { get; }
	protected StateMachine _stateMachine;

	protected State _parentState;
	protected State _childState;

	public State(StateMachine stateMachine)
	{
		_stateMachine = stateMachine;
	}

	public abstract void Enter();
	public abstract void Exit();
	public abstract void UpdateState();
	public virtual void FixedUpdate() { }

	public void UpdateStates()
	{
		UpdateState();
		if (_childState != null) _childState.UpdateState();
	}

	public void SetState(State state)
	{
		if (_rootState) SetChildState(state);
		else
		{
			Exit();
			_parentState.SetChildState(state);
		}
		state.Enter();
	}

	public void SetChildState(State state)
	{
		_childState = state;
		state.SetParentState(this);
	}

	public void SetParentState(State state)
	{
		if (_rootState) _stateMachine.SetState(this);
		else _parentState = state;
	}
}