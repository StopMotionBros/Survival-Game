using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
	[SerializeField] ItemData _item;
	[SerializeField] int _amount;
	[SerializeField] int _condition;

	public void Interact(IInteractor interactor)
	{
		PlayerController player = interactor as PlayerController;
		if (player.Inventory.AddItem(_item, _amount, out int remainder, _condition))
		{
			_amount = remainder;
			if (_amount <= 0) Destroy(gameObject);
		}
	}
}
