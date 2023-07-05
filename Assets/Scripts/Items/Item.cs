using UnityEngine;

public abstract class Item : MonoBehaviour
{
	public virtual void Equip()
	{
		gameObject.SetActive(true);
	}

	public virtual void Unequip()
	{
		gameObject.SetActive(false);
	}
}