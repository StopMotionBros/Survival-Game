using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : MonoBehaviour
{
	[SerializeField] Player _player;
	PlayerInventory _inventory;

	Dictionary<UIItemSlot, Item> _items = new();
	Item _equippedItem;
	UIItemSlot _equippedSlot;

	void Awake()
	{
		_inventory = _player.Inventory;
		_inventory.OnItemAdded += AddItem;
		_inventory.OnSlotDisconnected += RemoveItem;
	}

	void OnDestroy()
	{
		_inventory.OnItemAdded -= AddItem;
		_inventory.OnSlotDisconnected -= RemoveItem;
	}

	void AddItem(UIItemSlot slot)
	{
		ItemData item = slot.Slot.Item;
		if (!item.Equippable) return;
		if (_items.ContainsKey(slot)) return;

		Item heldItem = Instantiate(item.ItemPrefab, transform);
		heldItem.transform.localPosition = Vector3.zero;
		heldItem.transform.localRotation = Quaternion.identity;
		heldItem.Initialize(_player, slot);
		heldItem.Unequip();

		_items.Add(slot, heldItem);
	}

	void RemoveItem(UIItemSlot slot)
	{
		if (!slot) return;
		if (_equippedSlot == slot)
		{
			Unequip();
			_equippedSlot = _inventory.Inventory.FindSlotWithItem(slot.Slot.Item);
			if (_equippedSlot) Equip(_equippedSlot);
		}
		_items.Remove(slot);
	}

	public void Equip(UIItemSlot slot)
	{
		if (!_items.TryGetValue(slot, out Item heldItem)) return;

		if (_equippedItem) Unequip();

		_equippedItem = heldItem;
		_equippedSlot = slot;
		_equippedItem.Equip();
		_equippedSlot.Equip();
	}

	public void Unequip()
	{
		if (!_equippedItem) return;

		_equippedItem.Unequip();
		_equippedSlot.Unequip();
		_equippedItem = null;
		_equippedSlot = null;
	}

	public bool IsItemEquipped(ItemData item) => _equippedItem == null ? false : _equippedItem.Data == item;
	public bool IsSlotEquipped(UIItemSlot slot) => _equippedSlot == slot;
}