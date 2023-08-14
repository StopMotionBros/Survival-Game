using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
	[SerializeField] PlayerController _player;
	[SerializeField] Transform _orientation;
    [SerializeField] float _sensitivity;

	float _xRot;
	Vector2 _mouseDelta;

	void Awake()
	{
		GameManager.ToggleCursor(false);
	}

	void Update()
	{
		_mouseDelta = _sensitivity * Time.deltaTime * GetMouseDelta();
		_xRot -= _mouseDelta.y;

		_xRot = Mathf.Clamp(_xRot, -90, 90);

		_orientation.Rotate(_mouseDelta.x * Vector3.up);
		transform.localRotation = Quaternion.Euler(_xRot, 0, 0);
	}

	Vector2 GetMouseDelta() => _player.PlayerControls.Camera.Delta.ReadValue<Vector2>();
}
