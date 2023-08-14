using UnityEngine;

public class PlayerSwimState : PlayerState
{
	protected override bool _rootState => true;

	float _speed;
	Rigidbody _rigidbody;
	Transform _camHolder;

	public PlayerSwimState(PlayerController stateMachine) : base(stateMachine)
	{
		_rigidbody = _player.Rigidbody;
		_speed = _player.SwimSpeed;
		_camHolder = _player.CameraHolder;
	}

	public override void Enter()
	{
		_rigidbody.useGravity = false;
		_rigidbody.drag = 5; 
	}

	public override void Exit()
	{
		_rigidbody.useGravity = true;
	}

	public override void UpdateState()
	{
		Vector3 inputDirection = _player.GetMoveInput().ToVector3XZ();
		_rigidbody.AddForce(_speed * Time.deltaTime * _camHolder.TransformDirection(inputDirection), ForceMode.Force);
	}
}
