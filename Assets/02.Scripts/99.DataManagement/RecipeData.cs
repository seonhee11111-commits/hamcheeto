using System.Collections.Generic;

[System.Serializable]
public struct RecipeIngredient
{
    public IngredientType type;
    public IngredientState state;
    public int count;
}

[System.Serializable]
public class RecipeData
{
    public string nameEng;
    public string nameKor;
    public int price;
    public List<RecipeIngredient> ingredientsList = new List<RecipeIngredient>();
}
