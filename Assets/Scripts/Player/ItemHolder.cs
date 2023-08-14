using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : MonoBehaviour
{
	[SerializeField] PlayerController _player;
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
		heldItem.Unequip();

		_items.Add(slot, heldItem);
	}

	void RemoveItem(UIItemSlot slot)
	{
		if (!slot) return;
		if (_equippedSlot == slot) Unequip();
		_items.Remove(slot);
	}

	public void Equip(UIItemSlot slot)
	{
		if (!_items.TryGetValue(slot, out Item heldItem)) return;

		if (_equippedItem) _equippedItem.Unequip();

		_equippedItem = heldItem;
		_equippedSlot = slot;
		_equippedItem.Equip();
	}

	public void Unequip()
	{
		if (!_equippedItem) return;

		_equippedItem.Unequip();
		_equippedItem = null;
		_equippedSlot = null;
	}

	public bool IsItemEquipped(ItemData item) => _equippedItem == null ? false : _equippedItem.Data == item;
	public bool IsItemEquipped(UIItemSlot slot) => _equippedSlot == slot;
}