using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
	public Inventory Inventory => _inventory;

	[SerializeField] Player _player;
    [SerializeField] Inventory _inventory;
	[SerializeField] GameObject _content;

	[Space]

	[SerializeField] UIContextMenu _contextMenu;
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
		_inventory.SetUser(_player.Controller.Orientation);

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
		if (_player.Controls == null) return;

		SubscribeInputs();
	}

	void OnDisable()
	{
		UnsubscribeInputs();
	}

	void SubscribeInputs()
	{
		_player.Controls.UI.ToggleInventory.performed += ToggleInventory;
	}

	void UnsubscribeInputs()
	{
		_player.Controls.UI.ToggleInventory.performed -= ToggleInventory;
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
			_player.Controls.Movement.Enable();
			_player.Controls.Camera.Enable();
			_player.Controls.Interaction.Enable();
			_crafting.CloseRecipe();
			_crafting.Toggle(false);
		}
		else
		{
			_player.Controls.Movement.Disable();
			_player.Controls.Camera.Disable();
			_player.Controls.Interaction.Enable();
		}
	}

	public Vector3 GetDropPosition() => 
		_player.Controller.Orientation.position + _player.Controller.Orientation.forward + Vector3.up;
}
