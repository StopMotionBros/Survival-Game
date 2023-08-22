using UnityEngine;

public class BuildingItem : Item
{
	[SerializeField] BuildData _buildData;
	Transform _cameraHolder;

	Transform _buildTarget;

	protected override void OnInitialize()
	{
		_cameraHolder = _player.Controller.CameraHolder;
	}

	void Update()
	{
		if (!_equipped) return;
		if (!_buildTarget) _buildTarget = Instantiate(_buildData.BuildObject).transform;

		if (Physics.Raycast(_cameraHolder.position, _cameraHolder.forward, out RaycastHit hit, 15, 1))
		{
			_buildTarget.position = hit.point;
			if (Input.GetMouseButtonDown(0))
			{
				_buildTarget = null;
				DecreaseAmount();
			}
		}
	}
}