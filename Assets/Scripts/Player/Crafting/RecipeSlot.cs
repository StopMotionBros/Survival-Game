using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeSlot : MonoBehaviour
{
	Ingredient _result;

	[SerializeField] TMP_Text _name;
	[SerializeField] TMP_Text _amount;
	[SerializeField] Image _icon;

	public void SetResult(Ingredient result)
	{
		_result = result;

		ItemData item = _result.Item;
		_name.SetText(item.Name);
		_amount.SetText(_result.Amount + "x");
		_icon.sprite = item.Icon;
	}
}