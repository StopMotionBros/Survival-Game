using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class UIItemSlot : Selectable
{
	Container _container;

	[SerializeField] TMP_Text _name;
	[SerializeField] Image _icon;
	[SerializeField] TMP_Text _amount;
	[SerializeField] TMP_Text _weight;
	[SerializeField] ProgressBar _condition;

	public SlotData Slot { get => _slot; set => _slot = value; }
	[SerializeField] SlotData _slot;

	public void Init(ItemData item, Container container, int amount = 1, int condition = -1)
	{
		if (!item) throw new Exception("Item cannot be null");

		_container = container;
		_slot = new SlotData(item, amount, condition);

		_condition.gameObject.SetActive(item.Degradable && !item.Stackable);
		_condition.Max = item.MaxCondition;

		_amount.enabled = item.Stackable;

		_container.AddSlot(this);

		UpdateSlot();
	}

	public override void OnSelect(BaseEventData eventData)
	{
		base.OnSelect(eventData);

		_container.SelectSlot(this);
	}

	public bool AddItem(ItemData item, int amount, int condition = -1, bool update = true)
	{
		if (!item) return false;
		if (!_slot.Item.Stackable && !_slot.Empty) return false;

		if (item.Degradable)
			if (condition < 0) condition = item.MaxCondition;

		if (_slot.Empty)
		{
			_slot.Item = item;
			_slot.Condition = Mathf.Clamp(condition, 0, item.MaxCondition);

			if (!item.Stackable)
			{
				_slot.Amount = 1;

				if (update) UpdateSlot();
				return true;
			}
		}

		if (item != _slot.Item) return false;

		bool addedItem = false;
		if (item.Stackable)
		{
			_slot.Amount += amount;
		}

		if (update) UpdateSlot();
		return addedItem;
	}

	public void Drop(int amount)
	{
		ItemPickup pickup = Instantiate(_slot.Item.DropPrefab, transform.root.position, Quaternion.identity);
		pickup.SetAmount(Mathf.Min(_slot.Amount, amount));

		_slot.Amount -= amount;

		_container.Drop(_slot.Item, amount);

		if (_slot.Amount <= 0) Clear();
		else UpdateSlot();
	}

	public void UpdateSlot()
	{
		if (_slot.Empty) Clear();
		else UpdateUI();
	}

	void UpdateUI()
	{
		_name.SetText(_slot.Item.Name);
		if (_amount.enabled) _amount.SetText("(" + _slot.Amount + ")");
		_weight.SetText(_slot.Weight + "[lbs]");
		_condition.Value = _slot.Condition;
	}

	public void Clear()
	{
		_container.RemoveSlot(this);
		Destroy(gameObject);
	}
}
[Serializable]
public struct SlotData
{
	public ItemData Item;

	public int Amount;
	public int Condition;
	public int Weight => Item ? Item.Weight * Amount : 0;

	public int MaxCondition => Item.MaxCondition;

	public bool Empty => Item == null || Amount <= 0;

	public SlotData(ItemData item, int amount, int condition)
	{
		if (item == null) throw new Exception("Item cannot be null.");

		Item = item;

		Amount = amount;
		Condition = Mathf.Clamp(condition, -1, item.MaxCondition);
	}

	public void Clear()
	{
		Item = null;
		Amount = 0;
		Condition = -1;
	}

	public void Swap(SlotData other)
	{
		SlotData b = other;

		Item = b.Item;
		Amount = b.Amount;
		Condition = b.Condition;
	}
}
