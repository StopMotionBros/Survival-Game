using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class UIItemSlot : Selectable, ISubmitHandler
{
    Container _container;

    [SerializeField] Image _icon;
    [SerializeField] TMP_Text _amount;
    [SerializeField] ProgressBar _condition;

    CursorSlot _cursor;
    
	public SlotData Slot { get => _slot; set => _slot = value; }
    [SerializeField] SlotData _slot;

	public void Init(ItemData item, Container container)
    {
        _cursor = CursorSlot.Instance;

        _container = container;
        _slot = new SlotData(item, 1, -1);

		if (item) _condition.Max = item.MaxCondition;

		UpdateSlot();
    }

	public void OnSubmit(BaseEventData eventData)
	{
		if (Swap(_cursor))
			_cursor.transform.position = transform.position;
	}

	public override void OnPointerDown(PointerEventData eventData)
	{
		base.OnPointerDown(eventData);

		if (Swap(_cursor))
		    _cursor.transform.position = transform.position;
	}

	public bool Swap(UIItemSlot other)
    {
        return _container.Swap(this, other);
    }

	public bool AddItem(ItemData item, int amount, out int remainder, int condition = -1)
    {
        remainder = amount;

        if (!item) return false;

        if (_slot.Full) return false;

        if (item.Degradable)
            if (condition == -1) condition = item.MaxCondition;

        if (_slot.Empty)
        {
            _slot.Item = item;
            _slot.Condition = Mathf.Clamp(condition, 0, item.MaxCondition);

            if (!item.Stackable)
			{
                remainder--;
				_slot.Amount = 1;

                UpdateSlot();
				return true;
            }
        }

        if (item != _slot.Item) return false;

        bool addedItem = false;
        if (item.Stackable)
        {
            if (_slot.Amount + amount <= _slot.MaxAmount)
            {
                remainder = 0;
                _slot.Amount += amount;

                addedItem = true;
            }
            else if (_slot.Amount + amount > _slot.MaxAmount)
            {
                remainder = remainder - _slot.MaxAmount - _slot.Amount;
                _slot.Amount = _slot.MaxAmount;

                addedItem = true;
            }
        }

        UpdateSlot();
        return addedItem;
    }

    public void UpdateSlot()
    {
        if (_slot.Empty)
        {
            Clear();

            _icon.enabled = false;
            _amount.enabled = false;
            _condition.gameObject.SetActive(false);
        }
        else
		{
			if (!_icon.enabled) _icon.enabled = true;
			if (!_amount.enabled) _amount.enabled = true;
            if (_slot.Item.Degradable && !_condition.gameObject.activeSelf) _condition.gameObject.SetActive(true);
			if (_condition.Max != _slot.MaxCondition) _condition.Max = _slot.MaxCondition;
            
            UpdateUI();
        }
    }

    void UpdateUI()
    {
		_icon.sprite = _slot.Item.Icon;

        if (_slot.Amount == 1) _amount.enabled = false;
        else if (!_amount.enabled) _amount.enabled = true;
		_amount.SetText(_slot.Amount.ToString());

        _condition.Value = _slot.Condition;
	}

    public void Clear()
    {
        _slot.Clear();
    }
}
[Serializable]
public struct SlotData
{
    public ItemData Item;

    public int Amount;
    public int Condition;

    public int MaxAmount => Item.MaxAmount;
    public int MaxCondition => Item.MaxCondition;

    public bool Empty => Item == null || Amount <= 0;
    public bool Full => Item != null && Amount == Item.MaxAmount;

	public SlotData(ItemData item, int amount, int condition)
	{
		Item = item;

        if (Item == null)
        {
            Amount = 0;
            Condition = -1;

            return;
        }

		Amount = Mathf.Clamp(amount, 0, item.MaxAmount);
		Condition = Mathf.Clamp(condition, 0, item.MaxCondition);

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
