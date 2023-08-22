using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Data/Item")]
public class ItemData : ScriptableObject
{
	#region Getters

	public string Name => _name;
	public string Description => _description;

	public int MaxCondition => _maxCondition;
	public int MaxStack => _maxStack;
	public int2 Size => _size;

	public Sprite Icon => _icon;

	public ItemPickup DropPrefab => _dropPrefab;
	public Item ItemPrefab => _itemPrefab;

	public bool Stackable => _stackable && !_degradable;
	public bool Degradable => _degradable;
	public bool Equippable => _equippable;
	public bool Usable => _usable;

	#endregion

	[SerializeField] ItemPickup _dropPrefab;
	[SerializeField] Item _itemPrefab;

	[Space]

	[SerializeField] bool _stackable;
	[SerializeField] bool _degradable;
	[SerializeField] bool _equippable;
	[SerializeField] bool _usable;

	[Space]

	[SerializeField] string _name;
	[SerializeField, TextArea(3, 10)] string _description;

	[Space]

	[SerializeField] int _maxCondition;
	[SerializeField] int _maxStack;

	[SerializeField] int2 _size;

	[Space]

	[SerializeField] Sprite _icon;
}
public enum ItemType
{
	NonStackable,
	Stackable,
	Degradable
}
