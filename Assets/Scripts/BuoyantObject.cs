using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BuoyantObject : MonoBehaviour
{
	Transform _transform;

	[SerializeField] Transform[] _floaters;

	[SerializeField] float _floatingForce;
	[SerializeField] float _density;

	[SerializeField] float _waterDrag = 3, _waterAngularDrag = 1;
	float _ogDrag, _ogAngularDrag;

	Rigidbody _rigidbody;
    Collider _waterCollider;

	float _floaterDivisor;
	float _waterSurface;

	float _ogMass;

	void Awake()
	{
		_transform = transform;

		_rigidbody = GetComponent<Rigidbody>();
		_ogMass = _rigidbody.mass;

		_ogDrag = _rigidbody.drag;
		_ogAngularDrag = _rigidbody.angularDrag;

		_floaterDivisor = 1f / _floaters.Length;
	}

	void FixedUpdate()
	{
		if (!_waterCollider) return;

		foreach (Transform floater in _floaters)
		{
			if (!_waterCollider.bounds.Contains(floater.position)) continue;

			if (floater.position.y < _waterSurface)
			{
				float depth = _waterSurface - floater.position.y;
				float clampedDepth = Mathf.Clamp01(depth);

				_rigidbody.AddForceAtPosition(_floaterDivisor * _floatingForce * clampedDepth * Vector3.up, floater.position, ForceMode.Force);

				_rigidbody.drag = Mathf.Max(depth, _waterDrag);
				_rigidbody.angularDrag = Mathf.Max(depth, _waterAngularDrag);
			}
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Water"))
		{
			_waterCollider = other;
			_waterSurface = _waterCollider.bounds.max.y;

			_rigidbody.SetDensity(_density);
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Water") && other == _waterCollider)
		{
			_waterCollider = null;

			_rigidbody.mass = _ogMass;

			_rigidbody.drag = _ogDrag;
			_rigidbody.angularDrag = _ogAngularDrag;

			_rigidbody.AddForce(0.5f * _ogMass * _rigidbody.velocity.y * Vector3.down, ForceMode.Impulse);
		}
	}

	void OnDrawGizmos()
	{
		if (_floaters.Length == 0) return;

		Gizmos.color = Color.green;
		foreach (Transform floater in _floaters)
		{
			Gizmos.DrawSphere(floater.position, 0.1f);
		}
	}
}
