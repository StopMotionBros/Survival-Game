using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "Data/Recipe")]
public class RecipeData : ScriptableObject
{
    public Ingredient[] Ingredients => _ingredients;
    public Ingredient Result => _result;

    [SerializeField] Ingredient[] _ingredients;
    [SerializeField] Ingredient _result;
}
[Serializable]
public struct Ingredient
{
    public ItemData Item;
    public int Amount;
}
