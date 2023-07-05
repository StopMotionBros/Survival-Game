using System;
using UnityEngine;

public class PlayerController : StateMachine, IInteractor
{
	#region Getters

	public Transform Tranform => _transform;
	public Transform CameraHolder => _cameraHolder;
	public PlayerControls PlayerControls => _controls;
	public Rigidbody Rigidbody => _rigidbody;
	public ItemHolder ItemHolder => _itemHolder;
	public float WalkSpeed => _walkSpeed;
	public float AirMultiplier => _airMultiplier;
	public float JumpForce => _jumpForce;
	public Container Inventory => _inventory;


	#endregion

	Transform _transform;

	PlayerControls _controls;

	[SerializeField] Rigidbody _rigidbody;
	[SerializeField] CapsuleCollider _collider;
	[SerializeField] Transform _cameraHolder;
	[SerializeField] Transform _orientation;

	[Space]

	[SerializeField] ItemHolder _itemHolder;

	[Space]

	[SerializeField] Container _inventory;

	[Space]

	[SerializeField] float _walkSpeed;
	[SerializeField] float _airMultiplier;
	[SerializeField] float _speed;

	[Space]

	[SerializeField] float _jumpForce;

	Vector2 _moveInput;
	Vector3 _moveDirection;

	[Space]

	[SerializeField] LayerMask _ground;

	public bool IsGrounded => Physics.CheckSphere(_transform.position, 0.2f, _ground);

	public PlayerGroundedState Grounded;
	public PlayerJumpState Jump;
	public PlayerFallState Fall;

	public PlayerIdleState Idle;
	public PlayerWalkState Walk;

	void Awake()
	{
		_transform = transform;

		_controls = new PlayerControls();
		_controls.Enable();

		Grounded = new PlayerGroundedState(this);
		Jump = new PlayerJumpState(this);
		Fall = new PlayerFallState(this);

		Idle = new PlayerIdleState(this);
		Walk = new PlayerWalkState(this);

		SetState(Grounded);
	}

	void Update()
	{
		CurrentState.UpdateStates();
	}

	void FixedUpdate()
	{
		CurrentState.FixedUpdate();
	}

	public void MovePlayer(float multiplier = 1)
	{
		_moveInput = GetMoveInput();
		_moveDirection = _moveInput.x * _orientation.right + _moveInput.y * _orientation.forward;

		_rigidbody.AddForce(_speed * multiplier * Time.deltaTime * _moveDirection, ForceMode.Force);
	}

	public Vector2 GetMoveInput() => _controls.Movement.Movement.ReadValue<Vector2>();

	public void SetSpeed(float speed) => _speed = speed;
}
