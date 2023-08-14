using UnityEngine;

public class Water : MonoBehaviour
{
	void OnTriggerEnter(Collider other)
	{
		if (other.attachedRigidbody)
		{
			other.attachedRigidbody.useGravity = false;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.attachedRigidbody)
		{
			other.attachedRigidbody.useGravity = true;
		}
	}
}
