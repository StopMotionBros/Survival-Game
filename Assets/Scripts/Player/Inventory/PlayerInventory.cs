using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
	public Inventory Inventory => _inventory;

	[SerializeField] PlayerController _player;
    [SerializeField] Inventory _inventory;
	[SerializeField] GameObject _content;

	[Space]

	[SerializeField] ContextMenu _contextMenu;
	[SerializeField] Crafting _crafting;

	bool _open;

    public Action<UIItemSlot> OnItemAdded;
	public Action<UIItemSlot> OnSlotDisconnected;
	public Action<UIItemSlot> OnSlotSelected;

	void Awake()
	{
        _inventory.OnItemAdded += i => OnItemAdded?.Invoke(i);
		_inventory.OnSlotDisconnected += i => OnSlotDisconnected?.Invoke(i);
		_inventory.OnSlotSelected += i => OnSlotSelected?.Invoke(i);

		_inventory.OnSlotRightClick += _contextMenu.SelectSlot;
		_inventory.SetUser(_player.Orientation);

		_crafting.SetContainer(this);
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

    public bool AddItem(ItemData item, int amount, out int remainder, int condition) => _inventory.AddItem(item, amount, out remainder, condition);

	public void ToggleInventory(bool open)
	{
		_open = open;
		_content.SetActive(_open);
		GameManager.ToggleCursor(_open);

		if (!open)
		{
			_player.PlayerControls.Movement.Enable();
			_player.PlayerControls.Camera.Enable();
			_player.PlayerControls.Interaction.Enable();
			_crafting.CloseRecipe();
			_crafting.Toggle(false);
		}
		else
		{
			_player.PlayerControls.Movement.Disable();
			_player.PlayerControls.Camera.Disable();
			_player.PlayerControls.Interaction.Enable();
		}
	}

	public Vector3 GetDropPosition() => _player.Orientation.position + _player.Orientation.forward + Vector3.up;
}
