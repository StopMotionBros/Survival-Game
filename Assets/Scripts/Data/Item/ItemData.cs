using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    #region Getters

    public string Name => _name;
    public int MaxAmount => _maxAmount;
    public int MaxCondition => _maxCondition;
    public Sprite Icon => _icon;

	public ItemPickup DropPrefab => _dropPrefab;
	public Item ItemPrefab  => _itemPrefab;

	public bool Stackable => _stackable;
	public bool Degradable => _degradable;
	public bool Equippable => _equippable;

	#endregion

	[SerializeField] ItemPickup _dropPrefab;
    [SerializeField] Item _itemPrefab;

    [Space]

    [SerializeField] bool _stackable;
    [SerializeField] bool _degradable;
    [SerializeField] bool _equippable;

    [Space]

    [SerializeField] string _name;
    [SerializeField, TextArea(3, 10)] string _description;

    [Space]

    [SerializeField] int _maxAmount;
    [SerializeField] int _maxCondition;

    [Space]

    [SerializeField] Sprite _icon;
}
public enum ItemType
{
    NonStackable,
    Stackable,
    Degradable
}
