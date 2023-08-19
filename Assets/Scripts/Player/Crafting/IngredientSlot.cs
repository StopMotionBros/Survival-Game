using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientSlot : MonoBehaviour
{
	Ingredient _ingredient;

	[SerializeField] Image _background;

	[SerializeField] TMP_Text _name;
	[SerializeField] TMP_Text _description;
	[SerializeField] TMP_Text _amount;
	[SerializeField] Image _icon;

	public void SetIngredient(Ingredient ingredient, bool hasIngredient)
	{
		_ingredient = ingredient;

		ItemData item = _ingredient.Item;
		_name.SetText(item.Name);
		_description.SetText(item.Description);
		_amount.SetText(_ingredient.Amount + "x");
		_icon.sprite = item.Icon;

		if (!hasIngredient) _background.color = new Color(0.58f, 0.1f, 0.1f);
	}
}