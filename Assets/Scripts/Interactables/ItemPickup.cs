using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
	[SerializeField] ItemData _item;
	[SerializeField] int _amount;
	[SerializeField] int _condition;

	[Space]

	[SerializeField] List<string> _extraData;

	public void Interact(IInteractor interactor)
	{
		if (interactor.GetType() != typeof(Player)) return;

		Player player = interactor as Player;
		if (player.Inventory.AddItem(_item, _amount, out int remainder, _condition))
		{
			_amount = remainder;
			if (_amount <= 0) Destroy(gameObject);
		}
	}

	public void SetAmount(int amount) => _amount = amount;
	public void SetCondition(int condition) => _condition = condition;

	public void AddData(string data) => _extraData.Add(data);
	public string GetData(int index) => index >= 0 && index < _extraData.Count ? _extraData[index] : "data not found";
}
