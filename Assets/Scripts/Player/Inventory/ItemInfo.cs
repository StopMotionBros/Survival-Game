using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class ItemInfo : MonoBehaviour
{
	[SerializeField] PlayerController _player;

	[SerializeField] TMP_Text _name;
	[SerializeField] TMP_Text _description;
	[SerializeField] Image _icon;

	[SerializeField] Slider _dropAmount;

	UIItemSlot _selectedSlot;

	public void SelectSlot(UIItemSlot slot)
	{
		ItemData item = slot.Slot.Item;

		_name.SetText(item.Name);
		_description.SetText(item.Description);
		_icon.sprite = item.Icon;

		_dropAmount.maxValue = slot.Slot.Amount;

		_selectedSlot = slot;
	}

	public void Equip()
	{
		if (!_selectedSlot) return;
		_player.ItemHolder.Equip(_selectedSlot.Slot.Item);
	}

	public void Drop()
	{
		if (!_selectedSlot) return;
		_selectedSlot.Drop((int)_dropAmount.value);
	}
}