using UnityEngine;

public abstract class Item : MonoBehaviour
{
	public ItemData Data => _data;

	[SerializeField] ItemData _data;


	public virtual void Equip()
	{
		gameObject.SetActive(true);
	}

	public virtual void Unequip()
	{
		gameObject.SetActive(false);
	}
}