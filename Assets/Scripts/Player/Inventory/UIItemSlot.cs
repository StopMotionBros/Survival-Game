using System;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIItemSlot : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
	Container _container;

	[SerializeField] RectTransform _details;
	[SerializeField] Image _icon;
	[SerializeField] TMP_Text _amount;
	[SerializeField] ProgressBar _condition;

	public RectTransform RectTransform => _rectTransform;
	RectTransform _rectTransform;

	public int X { get; private set; }
	public int Y { get; private set; }
	public int Width { get; private set; }
	public int Height { get; private set; }

	public Direction2D Direction => _direction;
	Direction2D _direction;

	public SlotData Slot { get => _slot; set => _slot = value; }
	[SerializeField] SlotData _slot;

	public void Init(ItemData item, Container container, int amount = 1, int condition = -1)
	{
		if (!item) throw new Exception("Item cannot be null");

		_container = container;
		_slot = new SlotData(item, amount, condition);

		_rectTransform = GetComponent<RectTransform>();
		_rectTransform.sizeDelta = container.SlotSize * new Vector2(item.Size.x, item.Size.y);
		_details.sizeDelta = _rectTransform.sizeDelta;

		_icon.sprite = _slot.Item.Icon;

		_condition.gameObject.SetActive(item.Degradable && !item.Stackable);
		_condition.Max = item.MaxCondition;

		_amount.enabled = item.Stackable;
	}

	public void SetContainer(Container container)
	{
		if (container == null) return;

		_container = container;
		transform.SetParent(_container.SlotContainer);
	}

	public void SetRect(int x, int y, int width, int height)
	{
		X = x;
		Y = y;
		Width = width;
		Height = height;
	}

	public void GetBounds(out int x, out int y, out int w, out int h)
	{
		_container.Grid.ToLocal(transform.position, out int posX, out int posY);
		int width = _slot.Item.Size.x;
		int height = _slot.Item.Size.y;

		x = 0;
		y = 0;
		w = 0;
		h = 0;

		switch (_direction)
		{
			case Direction2D.North:
				x = posX;
				y = posY;
				w = width;
				h = height;
				break;
			case Direction2D.West:
				x = posX;
				y = posY;
				w = height;
				h = width;
				break;
			case Direction2D.South:
				x = posX;
				y = posY;
				w = width;
				h = height;
				break;
			case Direction2D.East:
				x = posX;
				y = posY;
				w = height;
				h = width;
				break;
		}
	}

	public void SetRotation(Direction2D direction)
	{
		_direction = direction;

		switch(_direction)
		{
			case Direction2D.North:
				_rectTransform.pivot = Vector2.zero;
				_details.sizeDelta = _container.SlotSize * new Vector2(Slot.Item.Size.x, Slot.Item.Size.y);
				break;
			case Direction2D.West:
				_rectTransform.pivot = Vector2.right;
				_details.sizeDelta = _container.SlotSize * new Vector2(Slot.Item.Size.y, Slot.Item.Size.x);
				break;
			case Direction2D.South:
				_rectTransform.pivot = Vector2.one;
				_details.sizeDelta = _container.SlotSize * new Vector2(Slot.Item.Size.x, Slot.Item.Size.y);
				break;
			case Direction2D.East:
				_rectTransform.pivot = Vector2.up;
				_details.sizeDelta = _container.SlotSize * new Vector2(Slot.Item.Size.y, Slot.Item.Size.x);
				break;
		}

		transform.rotation = Quaternion.Euler(0, 0, -90 * (int)_direction);
		_details.rotation = Quaternion.identity;
	}

	public void Rotate()
	{
		_direction = (Direction2D)((1 + (int)_direction) % 4);
		SetRotation(_direction);
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left) _container.SelectSlot(this);
		if (eventData.button == PointerEventData.InputButton.Right) _container.RightClickSlot(this);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		_container.HoverSlot(this);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		_container.StopHoverSlot(this);
	}

	public bool AddItem(ItemData item, int amount, out int remainder, int condition = -1)
	{
		remainder = amount;

		if (!item) return false;
		if (!_slot.Item.Stackable && !_slot.Empty) return false;

		if (item.Degradable)
			if (condition < 0) condition = item.MaxCondition;

		if (_slot.Empty)
		{
			_slot.Item = item;
			_slot.Condition = math.clamp(condition, 0, item.MaxCondition);

			if (!item.Stackable)
			{
				remainder -= 1;
				_slot.Amount = 1;

				UpdateSlot();
				return true;
			}
		}

		if (item != _slot.Item) return false;

		bool addedItem;
		
		if (_slot.Amount == item.MaxStack) addedItem = false;
		else if (_slot.Amount + amount > item.MaxStack)
		{
			remainder = _slot.Amount + amount - item.MaxStack;
			_slot.Amount = item.MaxStack;
			addedItem = true;
		}
		else
		{
			remainder = 0;
			_slot.Amount += amount;
			addedItem = true;
		}

		if (addedItem)
		{
			UpdateSlot();
			return true;
		}
		return false;
	}

	public bool RemoveItem(int amount, out int remainder)
	{
		remainder = amount;

		if (_slot.Empty) return false;

		if(_slot.Amount - amount >= 0)
		{
			remainder = 0;
			_slot.Amount -= amount;
		}
		else
		{
			remainder = _slot.Amount - remainder;
			_slot.Amount = 0;
		}

		UpdateSlot();
		return true;
	}

	public bool Drop(int amount, Vector3 position, out int remainder)
	{
		if (!RemoveItem(amount, out remainder)) return false;
		
		int amt = amount - remainder;

		ItemPickup pickup = Instantiate(_slot.Item.DropPrefab, position, Quaternion.identity);
		pickup.SetAmount(math.min(_slot.Amount, amt));

		UpdateSlot();
		return true;
		
	}

	public void UpdateSlot()
	{
		if (_slot.Empty) Clear();
		else UpdateUI();
	}

	void UpdateUI()
	{
		if (_amount.enabled) _amount.SetText("(" + _slot.Amount + ")");
		_condition.Value = _slot.Condition;
	}

	public void Clear()
	{
		_container.DisconnectSlot(this);
		Destroy(gameObject);
	}
}
[Serializable]
public struct SlotData
{
	public ItemData Item;

	public int Amount;
	public int Condition;

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
