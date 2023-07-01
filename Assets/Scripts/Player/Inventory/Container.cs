using System;
using UnityEngine;

public class Container : MonoBehaviour
{
    [SerializeField] UIItemSlot[] _slots;
	[SerializeField] UIItemSlot _slotPrefab;
	[SerializeField] Transform _slotContainer;

	[SerializeField] int _slotCount;

	[SerializeField] bool _onlyAllowEquipables;

	public Action<ItemData> OnItemAdded;
	
	void Awake()
	{
		_slots = new UIItemSlot[_slotCount];
		for (int i = 0; i < _slotCount; i++)
		{
			UIItemSlot slot;
			slot = Instantiate(_slotPrefab, _slotContainer);
			slot.Init(null, this);

			_slots[i] = slot;
		}
	}

	public bool Swap(UIItemSlot a, UIItemSlot b)
	{
		if (a == null || b == null) return false;

		if (_onlyAllowEquipables && ((!a.Slot.Empty && !a.Slot.Item.Equippable) || (!b.Slot.Empty && !b.Slot.Item.Equippable)))
			return false;

		SlotData aData = a.Slot;

		a.Slot = b.Slot;
		b.Slot = aData;

		a.UpdateSlot();
		b.UpdateSlot();

		return true;
	}

	public bool AddItem(ItemData item, int amount, out int remainder, int condition = -1)
	{
		remainder = amount;

		if (_onlyAllowEquipables && !item.Equippable) return false;

		int nextAmount;

		if (item.Degradable) 
			if (condition <= -1) 
				condition = item.MaxCondition;

		bool addedItem = false;
		foreach (UIItemSlot slot in _slots)
		{
			nextAmount = remainder;
			addedItem = slot.AddItem(item, nextAmount, out remainder, condition);
			if (remainder == 0) break;
		}

		if (addedItem) OnItemAdded?.Invoke(item);

		return addedItem;
	}
}
