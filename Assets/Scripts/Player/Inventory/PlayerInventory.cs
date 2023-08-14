using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
	public Container Container => _container;

	[SerializeField] PlayerController _player;
    [SerializeField] Container _container;
	[SerializeField] GameObject _content;

	[Space]

	[SerializeField] ContextMenu _contextMenu;

	bool _open;

    public Action<UIItemSlot> OnItemAdded;
	public Action<UIItemSlot> OnSlotDisconnected;
	public Action<UIItemSlot> OnSlotSelected;

	void Awake()
	{
        _container.OnItemAdded += i => OnItemAdded?.Invoke(i);
		_container.OnSlotDisconnected += i => OnSlotDisconnected?.Invoke(i);
		_container.OnSlotSelected += i => OnSlotSelected?.Invoke(i);

		_container.OnSlotRightClick += _contextMenu.SelectSlot;
		_container.SetUser(_player.Orientation);
	}

	void Start()
	{
		SubscribeInputs();

		ToggleInventory(false);
	}

	#region Input Setup
	void OnEnable()
	{
		if (_player.PlayerControls == null) return;

		SubscribeInputs();
	}

	void OnDisable()
	{
		UnsubscribeInputs();
	}

	void SubscribeInputs()
	{
		_player.PlayerControls.UI.ToggleInventory.performed += ToggleInventory;
	}

	void UnsubscribeInputs()
	{
		_player.PlayerControls.UI.ToggleInventory.performed -= ToggleInventory;
	}
	#endregion

	public void ToggleInventory(InputAction.CallbackContext context)
    {
		_open = !_open;
		ToggleInventory(_open);
    }

    public bool AddItem(ItemData item, int amount, out int remainder, int condition) => _container.AddItem(item, amount, out remainder, condition);

	public void ToggleInventory(bool open)
	{
		_open = open;
		_content.SetActive(_open);
		GameManager.ToggleCursor(_open);

		if (!open)
		{
			_player.PlayerControls.Movement.Enable();
			_player.PlayerControls.Camera.Enable();
		}
		else
		{
			_player.PlayerControls.Movement.Disable();
			_player.PlayerControls.Camera.Disable();
		}
	}
}
