using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : MonoBehaviour
{
	[SerializeField] PlayerController _player;
	Container _inventory;

	Dictionary<ItemData, Item> _items = new();

	void Awake()
	{
		_inventory = _player.Inventory;
		_inventory.OnItemAdded += AddItem;
	}

	void AddItem(ItemData item)
	{
		if (!item.Equippable) return;
		if (_items.ContainsKey(item)) return;

		Item newItem = Instantiate(item.ItemPrefab, transform);
		newItem.transform.localPosition = Vector3.zero;
		newItem.transform.localRotation = Quaternion.identity;

		_items.Add(item, newItem);
	}
}