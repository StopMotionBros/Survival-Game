using UnityEngine;

public class Axe : Item
{
	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Damage(20);
		}
	}
}