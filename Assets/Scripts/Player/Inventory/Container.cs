using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

public class Container : MonoBehaviour
{
	public Transform SlotContainer => _slotContainer;
	public float SlotSize => _slotSize;

	Transform _user;

	Canvas _canvas;

	[SerializeField] List<UIItemSlot> _slots;
	[SerializeField] UIItemSlot _slotPrefab;
	[SerializeField] Transform _slotContainer;

	[SerializeField] Tooltip _tooltip;

	[SerializeField] int _xSize, _ySize;
	[SerializeField] float _slotSize;
	public Grid2D<SlotData?> Grid => _grid;
	Grid2D<SlotData?> _grid;

	UIItemSlot _selectedSlot;
	Vector3 _selectedPosition;
	Direction2D _selectedDirection;

	public Action<UIItemSlot> OnItemAdded;
	public Action<UIItemSlot> OnSlotDisconnected;
	public Action<UIItemSlot> OnSlotSelected;
	public Action<UIItemSlot> OnSlotRightClick;
	public Action<UIItemSlot> OnSlotHover;
	public Action<UIItemSlot> OnSlotStopHover;
	
	public void Awake()
	{
		_canvas = GetComponentInParent<Canvas>();

		_grid = new(_slotContainer.position, _xSize, _ySize, _slotSize, Vector2.zero, null);
		RectTransform rect = GetComponent<RectTransform>();
		rect.sizeDelta = _slotSize * new Vector2(_xSize, _ySize) + (50 * Vector2.up);

		OnSlotHover += ShowTooltip;
		OnSlotStopHover += i => _tooltip.Hide();
	}

	void Update()
	{
		if (!_selectedSlot) return;

		_selectedSlot.transform.position = _grid.GetCellPosGlobal(Input.mousePosition - _selectedPosition);
		if (Input.GetKeyDown(KeyCode.R))
		{
			_selectedSlot.Rotate();
		}
		if (Input.GetMouseButtonUp(0))
		{
			_canvas.sortingOrder = 0;
			_selectedSlot.GetBounds(out int x, out int y, out int w, out int h);

			if (!_grid.InsideGrid(x, y) && !_grid.InsideGrid(x + w, y + h))
			{
				EventSystem es = EventSystem.current;
				PointerEventData pointerData = new PointerEventData(es);
				pointerData.position = Input.mousePosition;
				List<RaycastResult> results = new();
				es.RaycastAll(pointerData, results);

				Container hoveredContainer = null;
				foreach (RaycastResult ray in results)
				{
					if (!ray.gameObject.CompareTag("Container")) continue;

					hoveredContainer = ray.gameObject.GetComponent<Container>();

					break;
				}

				if(hoveredContainer)
				{
					_selectedSlot.SetContainer(hoveredContainer);
					if (hoveredContainer.TryAddSlot(_selectedSlot))
					{
						DisconnectSlot(_selectedSlot);
						_selectedSlot = null;
						return;
					}
					else
					{
						_selectedSlot.SetContainer(this);
						AddSlot(_selectedSlot);
						_selectedSlot.SetRotation(_selectedDirection);
						_selectedSlot = null;
						return;
					}
				}
			}

			if (TryAddSlot(_selectedSlot))
			{
				_selectedSlot = null;
			}
			else
			{
				AddSlot(_selectedSlot);
				_selectedSlot.SetRotation(_selectedDirection);
				_selectedSlot = null;
			}
		}
	}

	void ShowTooltip(UIItemSlot slot)
	{
		ItemData data = slot.Slot.Item;
		_tooltip.Show();
		_tooltip.Write(data.Name, data.Description);
	}

	public void SetUser(Transform user) => _user = user;

	public bool AddItem(ItemData item, int amount, out int remainder, int condition = -1)
	{
		remainder = amount;

		if (amount < 0) return false;

		if (!CheckForSpace(item.Size, out int x, out int y, out Direction2D direction)) return false;

		if (item.Degradable)
			if (condition < 0)
				condition = item.MaxCondition;

		UIItemSlot addedSlot = null;

		bool emptiedStack = false;
		if (item.Stackable)
		{
			if (_slots.Count > 0)
			{
				foreach (UIItemSlot slot in _slots)
				{
					if (!slot.Slot.Item.Stackable) continue;

					if (IncreaseSlotAmount(slot, item, remainder, out remainder, condition))
					{
						if (remainder <= 0)
						{
							addedSlot = slot;
							emptiedStack = true;
							break;
						}
					}
				}
			}
		}

		if (!emptiedStack)
		{
			addedSlot = CreateSlot(item, 0, condition);
			addedSlot.SetRotation(direction);
			
			IncreaseSlotAmount(addedSlot, item, amount, out remainder, condition);
			addedSlot.GetBounds(out _, out _, out int w, out int h);
			addedSlot.SetRect(x, y, w, h);

			AddSlot(addedSlot);

			if (remainder > 0) AddItem(item, remainder, out remainder, condition);

			addedSlot.UpdateSlot();
			emptiedStack = true;
		}

		if (emptiedStack) OnItemAdded?.Invoke(addedSlot);
		return emptiedStack;
	}

	UIItemSlot CreateSlot(ItemData item, int amount = 1, int condition = -1)
	{
		UIItemSlot slot = Instantiate(_slotPrefab, _slotContainer);
		slot.Init(item, this, amount, condition);
		return slot;
	}

	bool IncreaseSlotAmount(UIItemSlot slot, ItemData item, int amount, out int remainder, int condition = -1) => 
		slot.AddItem(item, amount, out remainder, condition);
	
	public bool CheckForSpace(int2 size) => CheckForSpace(size, out _, out _, out _);
	public bool CheckForSpace(int x, int y, int w, int h)
	{
		if (!_grid.InsideGrid(x, y) || !_grid.InsideGrid(x + w - 1, y + h - 1)) return false;
		for (int sX = x; sX < x + w; sX++)
		{
			for (int sY = y; sY < y + h; sY++)
			{
				if (_grid.GetCellLocal(sX, sY) != null) return false;
			}
		}

		return true;
	}
	public bool CheckForSpace(int2 size, out int freeX, out int freeY, out Direction2D direction)
	{
		freeX = 0;
		freeY = 0;
		direction = Direction2D.North;		

		int posX, posY;

		bool blocked;
		bool isSquare = size.x == size.y;
		for (int y = _ySize - 1; y >= 0; y--)
		{
			for (int x = 0; x < _xSize; x++)
			{
				Direction2D dir = Direction2D.North;
				blocked = false;
				for (int sx = 0; sx < size.x; sx++)
				{
					for (int sy = 0; sy < size.y; sy++)
					{
						posX = x + sx;
						posY = y + sy;

						if (_grid.InsideGrid(posX, posY))
							if (_grid.GetCellLocal(posX, posY) == null) continue;

						blocked = true;
						break;
					}
					if (blocked) break;
				}
				if (!isSquare && blocked)
				{
					blocked = false;
					for (int sx = 0; sx < size.y; sx++)
					{
						for (int sy = 0; sy < size.x; sy++)
						{
							posX = x + sx;
							posY = y + sy;

							if (_grid.InsideGrid(posX, posY))
								if (_grid.GetCellLocal(posX, posY) == null) continue;

							blocked = true;
							break;
						}
						if (blocked) break;
					}
					if (!blocked) dir = Direction2D.West;
				}

				if(!blocked)
				{
					freeX = x;
					freeY = y;
					direction = dir;

					return true;
				}
			}
		}

		return false;
	}

	public void SelectSlot(UIItemSlot slot)
	{
		if (_selectedSlot) return;

		_selectedSlot = slot;

		_canvas.sortingOrder = 99;
		_tooltip.Hide();
		RemoveSlot(_selectedSlot);

		_selectedSlot.transform.SetParent(_canvas.transform);

		_selectedPosition = Input.mousePosition - _selectedSlot.transform.position;
		_selectedDirection = _selectedSlot.Direction;

		OnSlotSelected?.Invoke(_selectedSlot);
	}

	public void RightClickSlot(UIItemSlot slot) => OnSlotRightClick?.Invoke(slot);

	public void HoverSlot(UIItemSlot slot)
	{
		if (!_selectedSlot) OnSlotHover?.Invoke(slot);
	}
	public void StopHoverSlot(UIItemSlot slot)
	{
		if (!_selectedSlot) OnSlotStopHover?.Invoke(slot);
	}
	
	public bool Drop(UIItemSlot slot, int amount) => Drop(slot, amount, out _);
	public bool Drop(UIItemSlot slot, int amount, out int remainder)
	{
		remainder = amount;

		if (slot == null) return false;
		if (amount <= 0) return false;
		if (slot.Slot.Empty) return false;

		if (slot.Drop(amount, _user.position + _user.forward + Vector3.up, out remainder))
		{
			if (remainder > 0)
			{
				UIItemSlot nextSlot = FindSlotWithItem(slot.Slot.Item);
				if (nextSlot) Drop(nextSlot, remainder, out remainder);
			}
			return true;
		}
		return false;
	}

	public UIItemSlot FindSlotWithItem(ItemData item)
	{
		foreach (UIItemSlot slot in _slots)
		{
			if (slot.Slot.Item == item)
				if(!slot.Slot.Empty)
					return slot;
		}
		return null;
	}

	public bool TryAddSlot(UIItemSlot slot)
	{
		if (slot == null) return false;

		slot.GetBounds(out int x, out int y, out int w, out int h);
		if (CheckForSpace(x, y, w, h))
		{
			slot.SetRect(x, y, w, h);
			AddSlot(slot);

			return true;
		}
		return false;
	}

	public void AddSlot(UIItemSlot slot)
	{
		int x = slot.X;
		int y = slot.Y;
		int w = slot.Width;
		int h = slot.Height;
		slot.transform.position = _grid.GetCellPosLocal(x, y);

		_grid.SetRange(x, y, w, h, slot.Slot);
		slot.transform.SetParent(_slotContainer);
		_slots.Add(slot);
	}
	public bool RemoveSlot(UIItemSlot slot)
	{
		if (!slot) return false;
		if (!_slots.Contains(slot)) return false;

		_grid.SetRange(slot.X, slot.Y, slot.Width, slot.Height, null);

		return true;
	}

	public bool DisconnectSlot(UIItemSlot slot)
	{
		if (!slot) return false;
		if (!_slots.Contains(slot)) return false;

		_grid.SetRange(slot.X, slot.Y, slot.Width, slot.Height, null);
		_slots.Remove(slot);

		OnSlotDisconnected?.Invoke(slot);

		return true;
	}

	//void OnDrawGizmos()
	//{
	//	if (_grid == null) return;

	//	_grid.DrawGrid(Color.white, Color.red);
	//}
}
