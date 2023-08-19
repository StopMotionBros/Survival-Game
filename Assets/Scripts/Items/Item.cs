using UnityEngine;

public abstract class Item : MonoBehaviour
{
	public ItemData Data => _data;
	[SerializeField] protected ItemData _data;

	protected UIItemSlot _slot;
	protected PlayerController _player;

	protected bool _equipped;

	public void Initialize(PlayerController player, UIItemSlot slot)
	{
		_player = player;
		_slot = slot;

		OnInitialize();
	}

	public virtual void Equip()
	{
		gameObject.SetActive(true);
		_equipped = true;
	}
	public virtual void Unequip() 
	{
		gameObject.SetActive(false);
		_equipped = false;
	}

	public void Damage(int damage = 1) => _slot.Damage(damage);
	public void IncreaseAmount(int amount = 1) => _slot.IncreaseAmount(amount);
	public void DecreaseAmount(int amount = 1) => _slot.DecreaseAmount(amount);

	protected virtual void OnInitialize() { }
}