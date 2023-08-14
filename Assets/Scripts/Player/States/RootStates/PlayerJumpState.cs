using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PlayerJumpState : PlayerState
{
	protected override bool _rootState => true;

	float _airMultiplier;
	float _jumpForce;
	Rigidbody _rigidbody;

	public PlayerJumpState(PlayerController stateMachine) : base(stateMachine)
	{
		_airMultiplier = _player.AirMultiplier;
		_jumpForce = _player.JumpForce;
		_rigidbody = _player.Rigidbody;
	}

	public async override void Enter()
	{
		await UniTask.WaitForFixedUpdate();

		_rigidbody.drag = 0;
		_rigidbody.AddForce(_jumpForce * Vector3.up, ForceMode.Impulse);
	}

	public override void Exit()
	{
	}

	public override void UpdateState()
	{
	}

	public override void FixedUpdate()
	{
		_player.Move(_airMultiplier);

		if (_rigidbody.velocity.y <= 0) _player.SetState(_player.Fall);
	}
}
