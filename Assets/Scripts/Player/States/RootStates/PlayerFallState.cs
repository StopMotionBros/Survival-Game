using UnityEngine;

public class PlayerFallState : PlayerState
{
	protected override bool _rootState => true;

	float _airMultiplier;

	public PlayerFallState(PlayerController stateMachine) : base(stateMachine)
	{
		_airMultiplier = _player.AirMultiplier;
	}

	public override void Enter()
	{
		_player.Rigidbody.drag = 0;
	}

	public override void Exit()
	{
		
	}

	public override void UpdateState()
	{
	}

	public override void FixedUpdate()
	{
		_player.MovePlayer(_airMultiplier);

		_player.Rigidbody.AddForce(5 * Vector3.down, ForceMode.Force);
		if (_player.IsGrounded) _player.SetState(_player.Grounded);
	}
}