using UnityEngine;

public class PlayerIdleState : PlayerState
{
	protected override bool _rootState => false;

	Vector2 _moveInput;

	public PlayerIdleState(PlayerController stateMachine) : base(stateMachine)
	{
	}

	public override void Enter()
	{
	}

	public override void Exit()
	{
	}

	public override void UpdateState()
	{
		_moveInput = _player.GetMoveInput();
		if (_moveInput != Vector2.zero) SetState(_player.Walk);
	}
}
