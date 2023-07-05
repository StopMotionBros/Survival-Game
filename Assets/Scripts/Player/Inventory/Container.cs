using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Container : MonoBehaviour
{
	[SerializeField] List<UIItemSlot> _slots;
	[SerializeField] UIItemSlot _slotPrefab;
	[SerializeField] Transform _slotContainer;

	[SerializeField] int _maxWeight;
	int _currentWeight;

	public Action<ItemData> OnItemAdded;

	[Space]

	[SerializeField] ItemInfo _itemInfo;

	public bool AddItem(ItemData item, int amount, out int remainder, int condition = -1)
	{
		remainder = amount;

		if (item.Degradable)
			if (condition <= -1)
				condition = item.MaxCondition;

		bool addedItem = false;
		if (_slots.Count > 0)
		{
			foreach (UIItemSlot slot in _slots)
			{
				if (item.Stackable)
				{
					for (int a = 0; a < amount; a++)
					{
						if (!CheckForSpace(item.Weight)) break;

						AddOne(slot, item, condition, ref remainder);
						addedItem = true;
					}
					slot.UpdateSlot();
				}
			}
		}

		if (!addedItem)
		{
			if (CheckForSpace(item.Weight))
			{
				UIItemSlot slot = CreateSlot(item, 1, condition);
				_currentWeight += item.Weight;
				remainder--;

				int nextAmount = remainder;

				if (item.Stackable)
				{
					for (int a = 1; a < amount; a++)
					{
						if (!CheckForSpace(item.Weight)) break;

						AddOne(slot, item, condition, ref remainder);
					}
				}
				else
				{
					if (nextAmount > 0 && CheckForSpace(item.Weight)) AddItem(item, nextAmount, out remainder, condition);
				}

				slot.UpdateSlot();
				addedItem = true;
			}
		}

		if (addedItem) OnItemAdded?.Invoke(item);

		return addedItem;
	}

	public void Drop(ItemData item, int amount)
	{
		_currentWeight -= item.Weight * amount;
	}

	UIItemSlot CreateSlot(ItemData item, int amount = 1, int condition = -1)
	{
		UIItemSlot slot = Instantiate(_slotPrefab, _slotContainer);
		slot.Init(item, this, amount, condition);
		return slot;
	}

	bool CheckForSpace(int weight)
	{
		return _currentWeight + weight <= _maxWeight;
	}

	void AddOne(UIItemSlot slot, ItemData item, int condition, ref int remainder)
	{
		slot.AddItem(item, 1, condition, false);
		_currentWeight += item.Weight;
		remainder--;
	}

	public void SelectSlot(UIItemSlot slot)
	{
		_itemInfo.SelectSlot(slot);
	}

	public void AddSlot(UIItemSlot slot) => _slots.Add(slot);
	public bool RemoveSlot(UIItemSlot slot) => _slots.Remove(slot);
}
