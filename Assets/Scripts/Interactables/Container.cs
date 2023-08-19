using UnityEngine;
using UnityEngine.InputSystem;

public class Container : MonoBehaviour, IInteractable
{
	[SerializeField] Inventory _inventory;
	PlayerController _player;

	void Start()
	{
		_player = PlayerController.Instance;
		SubscribeInputs();
	}

	#region Input Setup
	void OnEnable()
	{
		if (_player?.PlayerControls == null) return;

		SubscribeInputs();
	}

	void OnDisable()
	{
		UnsubscribeInputs();
	}

	void SubscribeInputs()
	{
		_player.PlayerControls.UI.ToggleInventory.performed += Close;
	}

	void UnsubscribeInputs()
	{
		_player.PlayerControls.UI.ToggleInventory.performed -= Close;
	}
	#endregion

	public void Interact(IInteractor interactor)
	{
		_inventory.gameObject.SetActive(true);
		_player.Inventory.ToggleInventory(true);
	}

	void Close(InputAction.CallbackContext context)
	{
		_inventory.gameObject.SetActive(false);
	}
}
