using UnityEngine;

public class Player : MonoBehaviour, IInteractor
{
	public static Player Instance;

	public PlayerController Controller => _controller;
	public PlayerAnimator Animator => _animator;
	public PlayerInventory Inventory => _inventory;
	public ItemHolder ItemHolder => _itemHolder;
	public PlayerControls Controls => _controls;

	[SerializeField] PlayerController _controller;
	[SerializeField] PlayerAnimator _animator;
	[SerializeField] PlayerInventory _inventory;
	[SerializeField] ItemHolder _itemHolder;
	PlayerControls _controls;

	void Awake()
	{
		Instance = this;

		_controls = new PlayerControls();
		_controls.Enable();
	}
}