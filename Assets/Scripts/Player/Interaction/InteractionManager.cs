using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionManager : MonoBehaviour
{
	[SerializeField] float _interactDistance;
	[SerializeField] PlayerController _player;

	[SerializeField] LayerMask _interactable;

	Transform _cameraHolder;

	void Start()
	{
		_cameraHolder = _player.CameraHolder;

		SubscribeInputs();
	}

	#region Input Setup
	void OnEnable()
	{
		if (_player.PlayerControls == null) return;

		SubscribeInputs();
	}

	void OnDisable()
	{
		UnsubscribeInputs();
	}

	void SubscribeInputs()
	{
		_player.PlayerControls.Interaction.Interact.performed += Interact;
	}

	void UnsubscribeInputs()
	{
		_player.PlayerControls.Interaction.Interact.performed -= Interact;
	}
	#endregion

	void Interact(InputAction.CallbackContext context)
	{
		if(Physics.Raycast(_cameraHolder.position, _cameraHolder.forward, out RaycastHit hit, _interactDistance, _interactable))
		{
			if (!hit.collider.CompareTag("Interactable")) return;

			if (!hit.collider.TryGetComponent(out IInteractable interactable)) return;

			interactable.Interact(_player);
		}
	}
}