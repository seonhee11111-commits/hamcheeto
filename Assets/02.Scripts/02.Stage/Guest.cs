using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Guest : MonoBehaviour
{
    public RecipeData currentOrder;
    public OrderUI orderUI;

    private HamFaceUIChange HamFaceUIChange;

    [SerializeField]
    private float moveDuration = 1.0f;

    [Header("Sprites")]
    [SerializeField] private SpriteRenderer guestSpriteRenderer;
    [SerializeField] private Sprite success;
    [SerializeField] private Sprite fail;

    private void Start()
    {
        orderUI = GetComponentInChildren<OrderUI>();
        currentOrder = RecipeManager.instance.Recipes[Random.Range(0, RecipeManager.instance.Recipes.Count)];
        HamFaceUIChange = FindFirstObjectByType<HamFaceUIChange>();
        guestSpriteRenderer = GetComponent<SpriteRenderer>();

        if (orderUI != null)
        {
            orderUI.UpdateOrderUI(currentOrder);
            //Debug.Log("Recipe Update ok");
        }
        else Debug.Log("orderUI null");
        //Debug.Log($"주문:{currentOrder.nameKor}");

        StartCoroutine(EntryRoutine());
    }

    private IEnumerator EntryRoutine()
    {
        Vector3 endPos = transform.position;
        Vector3 startPos = endPos + Vector3.down * 2f;

        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / moveDuration);
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        transform.position = endPos;

        if (orderUI != null) orderUI.ShowOrderUI(currentOrder);
    }

    public void ReceiveToast(Packed_Toast Toast)
    {
        bool isCorrect = CompareRecipe(Toast.toastData, currentOrder.ingredientsList);

        if (isCorrect)
        {
            //Debug.Log("완벽한 토스트!");
            GameManager.instance.AddGold(currentOrder.price);
            SoundManager.Instance.PlaySFX(SFXType.MeowHappy);
            SoundManager.Instance.PlaySFX(SFXType.ToastSuccess);
            if (!GameManager.instance.recentOrderSuccess)
            {
                GameManager.instance.recentOrderSuccess = true;
            }
            else
            {
                GameManager.instance.AddGold(1000);
                GameManager.instance.ShowComboText();
            }
            HamFaceUIChange.TriggerSuccess();
            guestSpriteRenderer.sprite = success;
        }
        else
        {
            //Debug.Log("실패한 토스트...");
            GameManager.instance.HPDown();
            SoundManager.Instance.PlaySFX(SFXType.MeowAngry);
            SoundManager.Instance.PlaySFX(SFXType.ToastFail);
            if (GameManager.instance.recentOrderSuccess)
            {
                GameManager.instance.recentOrderSuccess = false;
            }
            HamFaceUIChange.TriggerFail();
            guestSpriteRenderer.sprite = fail;
        }

        if (orderUI != null) orderUI.HideOrderUI();       

        Destroy(Toast.gameObject);

        StartCoroutine(GuestExitRoutine());

        //Destroy(this.gameObject);
    }

    private IEnumerator GuestExitRoutine()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.right * 5f;

        float elapsed = 0f;
        float exitDuration = 0.6f;

        while (elapsed < exitDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / exitDuration);
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        if (GuestManager.instance != null)
        {
            GuestManager.instance.CallNextGuest();
        }

        Destroy(this.gameObject);
    }


    private bool CompareRecipe(Dictionary<(IngredientType, IngredientState), int> submittedData, List<RecipeIngredient> orderList)
    {
        Dictionary<(IngredientType,IngredientState),int> orderDict = new Dictionary<(IngredientType, IngredientState), int>();
        foreach (var ing in orderList)
        {
            var key = (ing.type, ing.state);
            if (orderDict.ContainsKey(key)) orderDict[key] += ing.count;
            else orderDict[key] = ing.count;
        }

        if (submittedData.Count != orderDict.Count) return false;

        foreach (var kvp in orderDict)
        {
            if(!submittedData.ContainsKey(kvp.Key) || submittedData[kvp.Key] != kvp.Value) return false;
        }

        return true;
    }

}
