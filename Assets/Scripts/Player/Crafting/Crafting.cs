using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Crafting : MonoBehaviour
{
	[SerializeField] RecipeData[] _recipes;
	[SerializeField] GameObject _content;

	[Space]

	[SerializeField] Transform _recipeContainer;
	[SerializeField] Transform _ingredienContainer;

	[Space]

	[SerializeField] Button _recipeButton;
	[SerializeField] GameObject _ingredientSlot;

	[Space]

	[SerializeField] GameObject _selectedRecipeUI;

	[Space]

	[SerializeField] TMP_Text _resultName;
	[SerializeField] TMP_Text _resultDescription;
	[SerializeField] TMP_Text _resultAmount;
	[SerializeField] Image _resultIcon;

	[Space]

	[SerializeField] Button _craftButton;

	PlayerInventory _inventory;

	RecipeData _selectedRecipe;
	
	void Awake()
	{
		foreach (RecipeData recipe in _recipes)
		{
			Button recipeButton = Instantiate(_recipeButton, _recipeContainer);
			RecipeSlot slot = recipeButton.GetComponent<RecipeSlot>();
			slot.SetResult(recipe.Result);

			recipeButton.onClick.AddListener(() => OpenRecipe(recipe));
		}
	}

	public void SetContainer(PlayerInventory inventory) => _inventory = inventory;

	void OpenRecipe(RecipeData recipe)
	{
		_selectedRecipe = recipe;
		_craftButton.interactable = true;
		if (_ingredienContainer.childCount > 0) _ingredienContainer.DestroyAllChildren();

		foreach (Ingredient ingredient in _selectedRecipe.Ingredients)
		{
			bool hasItem = _inventory.Inventory.HasItem(ingredient.Item, ingredient.Amount);
			IngredientSlot ingredientObj = Instantiate(_ingredientSlot, _ingredienContainer).GetComponent<IngredientSlot>();
			ingredientObj.SetIngredient(ingredient, hasItem);
			if (!hasItem) _craftButton.interactable = false;
		}

		Ingredient result = _selectedRecipe.Result;
		_resultName.SetText(result.Item.Name);
		_resultDescription.SetText(result.Item.Description);
		_resultIcon.sprite = result.Item.Icon;
		_resultAmount.SetText(result.Amount + "x");

		_selectedRecipeUI.SetActive(true);
	}

	public void CloseRecipe()
	{
		_selectedRecipe = null;
		_selectedRecipeUI.SetActive(false);
	}

	public void Craft()
	{
		Ingredient result = _selectedRecipe.Result;
		foreach (Ingredient ingredient in _selectedRecipe.Ingredients)
		{
			_inventory.Inventory.RemoveItem(ingredient.Item, ingredient.Amount);
		}
		_inventory.Inventory.AddItem(result.Item, result.Amount, out int remainder, result.Item.MaxCondition);

		if (remainder > 0)
		{
			ItemPickup pickup = Instantiate(result.Item.DropPrefab, _inventory.GetDropPosition(), Quaternion.identity);
			pickup.SetAmount(remainder);
		}

		OpenRecipe(_selectedRecipe);
	}

	public void Toggle() => Toggle(!_content.activeSelf);
	public void Toggle(bool enabled) => _content.SetActive(enabled);
}
