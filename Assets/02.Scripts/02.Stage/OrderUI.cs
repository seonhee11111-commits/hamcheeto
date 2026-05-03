using TMPro;
using UnityEngine;
using System.Text;
using System.Collections;

public class OrderUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI orderTXT;

    private RectTransform rect;
    private Vector2 showPos;
    private Vector2 hidePos;

    private void Awake()
    {
        rect= GetComponent<RectTransform>();
        showPos = rect.anchoredPosition;
        hidePos = showPos + Vector2.up * 500f;
        rect.anchoredPosition = hidePos;
    }

    private void Start()
    {
        Canvas myCanvas = GetComponent<Canvas>();
        if (myCanvas != null)
        {
            myCanvas.worldCamera = Camera.main;
            myCanvas.sortingOrder = -1;
        }
    }

    public void ShowOrderUI(RecipeData order)
    {
        UpdateOrderUI(order);
        StopAllCoroutines();
        StartCoroutine(SlideRoutine(hidePos, showPos));
    }

    public void HideOrderUI()
    {
        StopAllCoroutines();
        StartCoroutine(SlideRoutine(rect.anchoredPosition, hidePos));
    }

    private IEnumerator SlideRoutine(Vector2 start, Vector2 end)
    {
        float elapsed = 0f;
        float duration = 0.2f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / duration);
            rect.anchoredPosition = Vector2.Lerp(start, end, t);
            yield return null;
        }
        rect.anchoredPosition = end;
    }

    public void UpdateOrderUI(RecipeData order)
    {
        if (orderTXT == null) return;

        StringBuilder sb = new StringBuilder();

        sb.AppendLine($"{order.nameKor}");

        foreach (var ing in order.ingredientsList)
        {
            string stateKor = GetStateKor(ing.state);
            string typeKor = GetTypeKor(ing.type);
            sb.AppendLine($"{stateKor}{typeKor}*{ing.count}");
        }

        orderTXT.text = sb.ToString();
    }

    private string GetStateKor(IngredientState state)
    {
        switch (state)
        {
            case IngredientState.Raw: return "생 ";
            case IngredientState.Cooked: return "구운 ";
            case IngredientState.Burnt: return "탄 ";
            default: return "";
        }
    }

    private string GetTypeKor(IngredientType type)
    {
        switch (type)
        {
            case IngredientType.Bread: return "빵";
            case IngredientType.Ham: return "햄";
            case IngredientType.Cheese: return "치즈";
            case IngredientType.Egg: return "계란";
            case IngredientType.Onion: return "양파";
            case IngredientType.Cabbage: return "양배추";
            case IngredientType.Pickle: return "피클";
            case IngredientType.Ketchup: return "케찹";
            case IngredientType.Mustard: return "머스타드";
            default: return type.ToString();
        }
    }

}
