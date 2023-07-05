using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : MonoBehaviour
{
	[SerializeField] PlayerController _player;
	Container _inventory;

	Dictionary<ItemData, Item> _items = new();
	Item _equippedItem;

	void Awake()
	{
		_inventory = _player.Inventory;
		_inventory.OnItemAdded += AddItem;
	}

	void AddItem(ItemData item)
	{
		if (!item.Equippable) return;
		if (_items.ContainsKey(item)) return;

		Item heldItem = Instantiate(item.ItemPrefab, transform);
		heldItem.transform.localPosition = Vector3.zero;
		heldItem.transform.localRotation = Quaternion.identity;
		heldItem.Unequip();

		_items.Add(item, heldItem);
	}

	public void Equip(ItemData item)
	{
		if (!_items.TryGetValue(item, out Item heldItem)) return;

		if (_equippedItem) _equippedItem.Unequip();

		_equippedItem = heldItem;
		_equippedItem.Equip();
	}
}