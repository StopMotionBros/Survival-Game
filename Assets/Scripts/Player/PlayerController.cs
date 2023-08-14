using UnityEngine;

public class PlayerController : StateMachine, IInteractor
{
	public static PlayerController Instance;

	#region Getters

	public Transform Tranform => _transform;

	public Transform CameraHolder => _cameraHolder;
	public PlayerControls PlayerControls => _controls;
	public Transform Orientation => _orientation;
	public Rigidbody Rigidbody => _rigidbody;
	public CapsuleCollider Collider => _collider;
	public ItemHolder ItemHolder => _itemHolder;

	public float WalkSpeed => _walkSpeed;
	public float AirMultiplier => _airMultiplier;
	public float JumpForce => _jumpForce;
	public float SwimSpeed => _swimSpeed;

	public LayerMask Ground => _ground;
	public PlayerInventory Inventory => _inventory;


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

	[SerializeField] PlayerInventory _inventory;

	[Space]

	[SerializeField] float _walkSpeed;
	[SerializeField] float _airMultiplier;
	[SerializeField] float _swimSpeed;
	[SerializeField] float _speed;

	[SerializeField] float _maxSlopeAngle;
	
	[Space]

	[SerializeField] float _jumpForce;

	Vector2 _moveInput;
	Vector3 _moveDirection;

	[Space]

	[SerializeField] LayerMask _ground;

	float _radius;

	bool _inWater;

	public bool IsGrounded => Physics.CheckSphere(_transform.position, 0.2f, _ground);

	public PlayerGroundedState Grounded;
	public PlayerJumpState Jump;
	public PlayerFallState Fall;
	public PlayerSwimState Swim;

	public PlayerIdleState Idle;
	public PlayerWalkState Walk;

	void Awake()
	{
		Instance = this;

		_transform = transform;

		_controls = new PlayerControls();
		_controls.Enable();

		Grounded = new PlayerGroundedState(this);
		Jump = new PlayerJumpState(this);
		Fall = new PlayerFallState(this);
		Swim = new PlayerSwimState(this);

		Idle = new PlayerIdleState(this);
		Walk = new PlayerWalkState(this);

		SetState(Grounded);

		_radius = _collider.radius;
	}

	void Update()
	{
		CurrentState.UpdateStates();

		if (_inWater)
		{
			if(!IsGrounded)
			{
				if (CurrentState != Swim) SetState(Swim);
			}
			else
			{
				if (CurrentState != Grounded) SetState(Grounded);
			}
		}
	}

	void FixedUpdate()
	{
		CurrentState.FixedUpdate();
	}

	public void Move(float multiplier = 1, bool ignoreSlopes = false)
	{
		_moveInput = GetMoveInput();
		_moveDirection = _moveInput.x * _orientation.right + _moveInput.y * _orientation.forward;

		bool onSlope = false;
		RaycastHit hit = new RaycastHit();
		if (!ignoreSlopes)
		{
			Vector3 position = transform.position + _radius * _moveDirection + (_radius * Vector3.up);
			bool raycast = Physics.Raycast(position, Vector3.down, out hit, _radius, _ground);
			if (raycast)
			{
				float angle = Vector3.Angle(hit.normal, Vector3.up);
				onSlope = angle <= _maxSlopeAngle && angle > 5;
			}
			Debug.DrawLine(position, position + _radius * Vector3.down);
		}

		_rigidbody.useGravity = !onSlope;
		if (!onSlope) _rigidbody.AddForce(_speed * multiplier * Time.deltaTime * _moveDirection, ForceMode.Force);
		else
		{
			_rigidbody.AddForce(_speed * multiplier * Time.deltaTime * Vector3.ProjectOnPlane(_moveDirection, hit.normal), ForceMode.Force);
		}
	}

	public Vector2 GetMoveInput() => _controls.Movement.Movement.ReadValue<Vector2>();

	public void SetSpeed(float speed) => _speed = speed;

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Water")) _inWater = true;
	}

	void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Water"))
		{
			SetState(Fall);
			_inWater = false;
		}
	}
}
