public abstract class PlayerState : State
{
	protected PlayerController _player;

	public PlayerState(PlayerController stateMachine) : base(stateMachine)
	{
		_player = stateMachine;
	}
}
