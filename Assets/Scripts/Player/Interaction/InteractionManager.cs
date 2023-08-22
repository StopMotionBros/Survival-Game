using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionManager : MonoBehaviour
{
	[SerializeField] float _interactDistance;
	[SerializeField] Player _player;

	[SerializeField] LayerMask _interactable;

	Transform _cameraHolder;

	void Start()
	{
		_cameraHolder = _player.Controller.CameraHolder;

		SubscribeInputs();
	}

	#region Input Setup
	void OnEnable()
	{
		if (_player.Controls == null) return;

		SubscribeInputs();
	}

	void OnDisable()
	{
		UnsubscribeInputs();
	}

	void SubscribeInputs()
	{
		_player.Controls.Interaction.Interact.performed += Interact;
	}

	void UnsubscribeInputs()
	{
		_player.Controls.Interaction.Interact.performed -= Interact;
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