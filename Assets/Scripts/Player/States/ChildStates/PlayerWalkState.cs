using UnityEngine;

public class PlayerWalkState : PlayerState
{
	protected override bool _rootState => false;

	float _speed;
	Vector2 _moveInput;

	public PlayerWalkState(PlayerController stateMachine) : base(stateMachine)
	{
		_speed = _player.WalkSpeed;
	}

	public override void Enter()
	{
		_player.SetSpeed(_speed);
	}

	public override void Exit()
	{
	}

	public override void UpdateState()
	{
		_moveInput = _player.GetMoveInput();
		if (_moveInput == Vector2.zero) SetState(_player.Idle);
	}
}
