using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Video;


public class RecipeManager : MonoBehaviour
{
    public static RecipeManager instance { get; private set; }

    [Header("RecipeCSVData")]
    [SerializeField] private TextAsset recipeCSV;

    [Header("Recipes")]
    public List<RecipeData> Recipes = new List<RecipeData>();

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        LoadRecipeCSV();
    }

    private void LoadRecipeCSV()
    {
        if (recipeCSV == null)
        { Debug.Log("레시피CSV null"); }

        string[] rows = recipeCSV.text.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        //\n(엔터) 기준으로 Row(줄) 단위 분리
        //RemoveEmptyEntries = 엑셀끝 비어있는 줄 무시하기

        for (int i = 1; i < rows.Length; i++)
        {
            //rows[0]은 Num, Name_Eng... 등 데이터라서 [1]부터 시작
            string row = rows[i].Trim();
            if (string.IsNullOrWhiteSpace(row)) continue;
            //칸이 빈칸이면 아래 코드 진행없이 다음 i로 건너뛰기

            string[] columns = row.Split(',');

            RecipeData newRecipe = new RecipeData();
            newRecipe.nameEng = columns[0];
            newRecipe.nameKor = columns[1];
            newRecipe.price = int.Parse(columns[3]);

            ParseIngredients(columns[2], newRecipe.ingredientsList);

            Recipes.Add(newRecipe);
        }
        //Debug.Log($"레시피 로드 완료: 총 {Recipes.Count}개");
    }

    private void ParseIngredients(string ingredientText, List<RecipeIngredient> targetList)
    {
        string[] items = ingredientText.Split('/');

        Dictionary<(IngredientType, IngredientState), int> tempDict = new Dictionary<(IngredientType, IngredientState), int>();

        foreach (string item in items)
        {
            List<(IngredientType, IngredientState)> parsedList = ConvertTextToIngredients(item.Trim());

            foreach (var parsed in parsedList)
            {
                if (tempDict.ContainsKey(parsed)) tempDict[parsed]++;
                else tempDict[parsed] = 1;
            }
        }

        foreach (var kvp in tempDict)
        {
            targetList.Add(new RecipeIngredient
            {
                type = kvp.Key.Item1,
                state = kvp.Key.Item2,
                count = kvp.Value
            });
        }
    }

    private List<(IngredientType, IngredientState)> ConvertTextToIngredients(string text)
    {
        List<(IngredientType, IngredientState)> results = new List<(IngredientType, IngredientState)>();

        if (text.Contains("케찹"))
        {
            results.Add((IngredientType.Ketchup, IngredientState.None));
            text = text.Replace("케찹", ""); // "케찹구운빵" -> "구운빵"
        }
        if (text.Contains("머스타드"))
        {
            results.Add((IngredientType.Mustard, IngredientState.None));
            text = text.Replace("머스타드", "");
        }

        switch (text)
        {
            case "빵": results.Add((IngredientType.Bread, IngredientState.Raw)); break;
            case "구운빵": results.Add((IngredientType.Bread, IngredientState.Cooked)); break;
            case "탄빵": results.Add((IngredientType.Bread, IngredientState.Burnt)); break;
            case "햄": results.Add((IngredientType.Ham, IngredientState.Raw)); break;
            case "구운햄": results.Add((IngredientType.Ham, IngredientState.Cooked)); break;
            case "탄햄": results.Add((IngredientType.Ham, IngredientState.Burnt)); break;
            case "치즈": results.Add((IngredientType.Cheese, IngredientState.Raw)); break;
            case "구운치즈": results.Add((IngredientType.Cheese, IngredientState.Cooked)); break;
            case "탄치즈": results.Add((IngredientType.Cheese, IngredientState.Burnt)); break;
            case "계란": results.Add((IngredientType.Egg, IngredientState.Raw)); break;
            case "구운계란": results.Add((IngredientType.Egg, IngredientState.Cooked)); break;
            case "탄계란": results.Add((IngredientType.Egg, IngredientState.Burnt)); break;
            case "양파": results.Add((IngredientType.Onion, IngredientState.Raw)); break;
            case "구운양파": results.Add((IngredientType.Onion, IngredientState.Cooked)); break;
            case "탄양파": results.Add((IngredientType.Onion, IngredientState.Burnt)); break;
            case "양배추": results.Add((IngredientType.Cabbage, IngredientState.Raw)); break;
            case "탄양배추": results.Add((IngredientType.Cabbage, IngredientState.Burnt)); break;
            case "피클": results.Add((IngredientType.Pickle, IngredientState.Raw)); break;
            case "탄피클": results.Add((IngredientType.Pickle, IngredientState.Burnt)); break;

            default:
                Debug.LogWarning($"알 수 없는 재료:{text} -> 맨빵으로 변환");
                results.Add((IngredientType.Bread, IngredientState.Raw)); break;
        }

        return results;
    }

}
