using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGroundedState : PlayerState
{
	protected override bool _rootState => true;

	public PlayerGroundedState(PlayerController stateMachine) : base(stateMachine)
	{
	}

	public override void Enter()
	{
		_player.Rigidbody.drag = 5;

		if (_player.GetMoveInput() == Vector2.zero) SetState(_player.Idle);
		else SetState(_player.Walk);

		_player.PlayerControls.Movement.Jump.started += Jump;
	}

	public override void Exit()
	{
		_player.PlayerControls.Movement.Jump.started -= Jump;
	}

	public override void UpdateState()
	{
	}

	public override void FixedUpdate()
	{
		_player.MovePlayer();

		if (!_player.IsGrounded) _player.SetState(_player.Fall);
	}

	void Jump(InputAction.CallbackContext context) => _player.SetState(_player.Jump);
}