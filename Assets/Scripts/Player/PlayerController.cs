using System;
using Cysharp.Threading.Tasks;
using KinematicCharacterController;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, ICharacterController
{
	[SerializeField] KinematicCharacterMotor _motor;
	[SerializeField] Player _player;

	Transform _transform;

	public Transform Orientation => _orientation;
	public Transform CameraHolder;

	public PlayerBaseState BaseState => _baseState;
	public PlayerMovementState MovementState => _movementState;

	[SerializeField] Transform _orientation;
	[SerializeField] Rigidbody _rigidbody;

	[SerializeField] PlayerBaseState _baseState;
	[SerializeField] PlayerMovementState _movementState;

	[SerializeField] float _walkSpeed;
	[SerializeField] float _runSpeed;
	[SerializeField] float _airSpeed;

	[SerializeField] float _jumpForce;

	[SerializeField] Vector3 _gravity;

	Vector3 _velocity;

	bool _jumping;

	void Awake()
	{
		_motor.CharacterController = this;
		_transform = transform;
	}

	#region Input Setup
	void OnEnable()
	{
		if (_player.Controls == null) return;

		SubscribeInputs();
	}

	void OnDisable()
	{
		UnsubscribeInputs();
	}

	void SubscribeInputs()
	{
		_player.Controls.Movement.Run.started += StartRun;
		_player.Controls.Movement.Run.canceled += StopRun;
		_player.Controls.Movement.Crouch.started += StartCrouch;
		_player.Controls.Movement.Crouch.canceled += StopCrouch;

		_player.Controls.Movement.Jump.started += Jump;
	}

	void UnsubscribeInputs()
	{
		_player.Controls.Movement.Run.started -= StartRun;
		_player.Controls.Movement.Run.canceled -= StopRun;
		_player.Controls.Movement.Crouch.started -= StartCrouch;
		_player.Controls.Movement.Crouch.canceled -= StopCrouch;

		_player.Controls.Movement.Jump.started -= Jump;
	}
	#endregion

	public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
	{
		currentRotation = _transform.rotation;
	}

	public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
	{
		Vector2 moveInput = GetMoveInput();
		Vector3 moveDirection = moveInput.x * _orientation.right + moveInput.y * _orientation.forward;
		if(_motor.GroundingStatus.IsStableOnGround)
		{
			float speed = 0;
			switch (_movementState)
			{
				case PlayerMovementState.Walking:
					speed = _walkSpeed;
					break;
				case PlayerMovementState.Running:
					speed = _runSpeed;
					break;
			}

			currentVelocity = speed * moveDirection;

			if (_jumping)
			{
				_motor.ForceUnground();

				currentVelocity += _jumpForce * _motor.GroundingStatus.GroundNormal;
				_jumping = false;
			}
		}
		else
		{
			currentVelocity += _airSpeed * moveDirection;
			currentVelocity += deltaTime * _gravity;
		}

		_velocity = currentVelocity;
	}

	void StartRun(InputAction.CallbackContext context) => _movementState = PlayerMovementState.Running;
	void StopRun(InputAction.CallbackContext context) => _movementState = PlayerMovementState.Walking;

	void StartCrouch(InputAction.CallbackContext context) => _baseState = PlayerBaseState.Crouching;
	void StopCrouch(InputAction.CallbackContext context) => _baseState = PlayerBaseState.Standing;

	void Jump(InputAction.CallbackContext context)
	{
		if (!_motor.GroundingStatus.IsStableOnGround) return;

		_jumping = true;
	}

	public Vector2 GetMoveInput() => _player.Controls.Movement.Movement.ReadValue<Vector2>();

	public void BeforeCharacterUpdate(float deltaTime)
	{
	}

	public void PostGroundingUpdate(float deltaTime)
	{
	}

	public void AfterCharacterUpdate(float deltaTime)
	{
	}

	public bool IsColliderValidForCollisions(Collider coll)
	{
		return true;
	}

	public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
	{
	}

	public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
	{
	}

	public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
	{
	}

	public void OnDiscreteCollisionDetected(Collider hitCollider)
	{
	}
}
public enum PlayerBaseState { Standing, Crouching }
public enum PlayerMovementState { Walking, Running }
