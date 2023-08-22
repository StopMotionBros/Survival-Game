using UnityEngine;
using UnityEngine.InputSystem;

public class Container : MonoBehaviour, IInteractable
{
	[SerializeField] Inventory _inventory;
	Player _player;

	void Start()
	{
		_player = Player.Instance;
		SubscribeInputs();
	}

	#region Input Setup
	void OnEnable()
	{
		if (_player?.Controls == null) return;

		SubscribeInputs();
	}

	void OnDisable()
	{
		UnsubscribeInputs();
	}

	void SubscribeInputs()
	{
		_player.Controls.UI.ToggleInventory.performed += Close;
	}

	void UnsubscribeInputs()
	{
		_player.Controls.UI.ToggleInventory.performed -= Close;
	}
	#endregion

	public void Interact(IInteractor interactor)
	{
		if (interactor.GetType() != typeof(Player)) return;

		_inventory.gameObject.SetActive(true);
		(interactor as Player).Inventory.ToggleInventory(true);
	}

	void Close(InputAction.CallbackContext context)
	{
		_inventory.gameObject.SetActive(false);
	}
}
