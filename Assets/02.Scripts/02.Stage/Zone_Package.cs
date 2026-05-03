using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.EventSystems;

public class Zone_Package : DropZone, IPointerDownHandler
{
    private List<Ingredients> toastStack = new List<Ingredients>();
    private BoxCollider2D myCollider;

    [Header("Stack Settings")]
    [SerializeField] private float stackOffsetY = 0.1f; // 쌓일 때 띄우는 y축 높이
    [SerializeField] private float baseColliderHeight; // 초기 collider 높이

    [Header("Prefabs")]
    [SerializeField] private GameObject packedToastPrefab;

    private void Awake()
    {
        myCollider = GetComponent<BoxCollider2D>();
        baseColliderHeight = myCollider.size.y;
    }

    public override bool CanAccept(IngredientType type, IngredientState state)
    {
        if (toastStack.Count == 0)
        {  return type == IngredientType.Bread;  }
        else if (type == IngredientType.Egg && state==IngredientState.EggShell) return false;
        else return true;
    }

    public override void DropAction(Ingredients ingredient)
    {
        ingredient.currentZone = this;

        Vector3 newPos = transform.position;
        newPos.y += toastStack.Count * stackOffsetY;
        ingredient.transform.position = newPos;

        ingredient.GetComponent<SpriteRenderer>().sortingOrder = 100 + toastStack.Count;

        ingredient.GetComponent<Collider2D>().enabled = false;
        ingredient.UpdateIngredientGaugeUI();
        toastStack.Add(ingredient);

        UpdateColliderBounds();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //아무것도 든 게 없을 때 재료 집어들기
        if (!CursorManager.instance.IsHandHolding) RemoveTopIngredient();
    }

    public void RemoveTopIngredient()
    {
        //맨 위 재료 다시 집어들기
        if (toastStack.Count == 0) return;

        int topIndex = toastStack.Count - 1;
        Ingredients topIngredient = toastStack[topIndex];

        toastStack.RemoveAt(topIndex);
        UpdateColliderBounds();

        //Collider 복구 및 커서에 따라가기
        topIngredient.GetComponent<Collider2D>().enabled = true;
        topIngredient.TryPickUp();
    }


    private void UpdateColliderBounds()
    {
        //스택 높이에 비례해 콜라이더 크기와 중심점을 위로 확장
        float addedHeight = toastStack.Count * stackOffsetY;
        myCollider.size = new Vector2(myCollider.size.x, baseColliderHeight + addedHeight);
        myCollider.offset = new Vector2(0, addedHeight / 2f);
    }

    public void TryPackToast()
    {
        if (toastStack.Count < 2) return;

        Ingredients bottom = toastStack[0];
        Ingredients top = toastStack[toastStack.Count - 1];

        if (bottom.myType == IngredientType.Bread && top.myType == IngredientType.Bread)
        {
            Vector3 spawnPos = transform.position + new Vector3(0, 0.25f, 0);

            GameObject packedObj = Instantiate(packedToastPrefab,spawnPos, Quaternion.identity);

            Packed_Toast packedToast = packedObj.GetComponent<Packed_Toast>();
            if (packedToast != null)
            {
                packedToast.InitializeData(toastStack);
            }

            SoundManager.Instance.PlaySFX(SFXType.PackToast, 0.8f);

            foreach (var ing in toastStack)
            {
                Destroy(ing.gameObject);
            }
            toastStack.Clear();

            UpdateColliderBounds();

            //Debug.Log("포장/초기화 완료");
        }
    }

    public Ingredients GetTopIngredient()
    {
        if (toastStack.Count > 0) return toastStack[toastStack.Count - 1];
        else return null;
    }
}
