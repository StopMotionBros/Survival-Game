using UnityEngine.UI;
using UnityEngine;

public class ContextMenu : MonoBehaviour
{
	[SerializeField] PlayerController _player;
	[SerializeField] Inventory _inventory;
	[SerializeField] Slider _dropAmount;

	[SerializeField] GameObject _dropModal;

	[SerializeField] GameObject _content;

	[Space]

	[SerializeField] GameObject _equipButton;
	[SerializeField] GameObject _unequipButton;
	[SerializeField] GameObject _useButton;
	[SerializeField] GameObject _assignButton;

	UIItemSlot _selectedSlot;

	public void SelectSlot(UIItemSlot slot)
	{
		_selectedSlot = slot;
		ItemData item = _selectedSlot.Slot.Item;
		transform.position = Input.mousePosition;

		_equipButton.SetActive(item.Equippable && !_player.ItemHolder.IsSlotEquipped(_selectedSlot));
		_unequipButton.SetActive(item.Equippable && _player.ItemHolder.IsSlotEquipped(_selectedSlot));
		_useButton.SetActive(item.Usable);
		_assignButton.SetActive(item.Equippable || item.Usable);

		Toggle(true);
	}

	public void Equip()
	{
		if (!_selectedSlot) return;
		_player.ItemHolder.Equip(_selectedSlot);
		Toggle(false);
	}

	public void Unequip()
	{
		if (!_selectedSlot) return;
		_player.ItemHolder.Unequip();
		Toggle(false);
	}

	public void ToggleDropModal(bool enabled)
	{
		_dropModal.SetActive(enabled);
		_dropAmount.value = 1;
		Toggle(false);

		if (!enabled) return;

		if (_selectedSlot.Slot.Amount <= 1) Drop();
		else _dropAmount.maxValue = _selectedSlot.Slot.Amount;
	}

	public void Drop()
	{
		if (!_selectedSlot) return;
		if (_inventory.Drop(_selectedSlot, (int)_dropAmount.value))
		{
			if (_player.ItemHolder.IsSlotEquipped(_selectedSlot)) _player.ItemHolder.Unequip();
		}
		_selectedSlot = null;
		ToggleDropModal(false);
	}

	public void Toggle(bool enabled)
	{
		_content.SetActive(enabled);
	}
}