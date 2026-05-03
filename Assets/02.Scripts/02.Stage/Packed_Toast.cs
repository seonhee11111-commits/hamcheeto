using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.RenderGraphModule;

public class Packed_Toast : ClickableObject
{
    private bool isHandHeld = false;
    private Camera mainCam;
    private Collider2D myCollider;
    private SpriteRenderer myRenderer;

    private Vector3 lastPos;
    private DropZone lastZone;
    public DropZone currentZone;
    private Coroutine returnCoroutine;
    private bool isFromContainer = false;

    public Dictionary<(IngredientType, IngredientState), int> toastData { get; private set; }
    = new Dictionary<(IngredientType, IngredientState), int>();
    //(종류,상태)가 key, (갯수)가 value

    private void Awake()
    {
        mainCam = Camera.main;
        myCollider = GetComponent<Collider2D>();
        myRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (isHandHeld)
        {
            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
            Vector3 screenPosWithZ = new Vector3(mouseScreenPos.x, mouseScreenPos.y, Mathf.Abs(mainCam.transform.position.z));
            transform.position = (Vector2)mainCam.ScreenToWorldPoint(screenPosWithZ);

            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                TryDrop();
            }
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (!isHandHeld && !CursorManager.instance.IsHandHolding)
        {
            isFromContainer = false;
            TryPickUp();
        }
    }


    private void TryDrop()
    {
        isHandHeld = false;
        //CursorManager.instance.SetCursorState(false);
        CursorManager.instance.IsHandHolding = false;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.zero);
        if (myCollider != null) myCollider.enabled = true;

        if (hit.collider != null)
        {
            Guest guest = hit.collider.GetComponent<Guest>();
            if (guest != null)
            {
                guest.ReceiveToast(this);
                return;
            }

            DropZone zone = hit.collider.GetComponent<DropZone>();
            if (zone is Zone_WasteCan wastecan)
            {
                SoundManager.Instance.PlaySFX(SFXType.Trash, 0.2f);
                Destroy(this.gameObject);
            }
        }

        DropFailure();
    }

    private void TryPickUp()
    {
        isHandHeld = true;
        //CursorManager.instance.SetCursorState(true);
        CursorManager.instance.IsHandHolding = true;
        lastPos = transform.position;
        lastZone = currentZone;
        currentZone = null;

        if (myCollider != null)
        {
            //들어올리면서 현재 객체의 콜라이더를 잠깐 끔 (Zone 인식 방해 방지)
            myCollider.enabled = false;
        }
        myRenderer.sortingOrder = 998;
    }

    private void DropFailure()
    {
        if (isFromContainer) Destroy(gameObject);
        else
        {
            Vector3 targetPos = lastPos;

            if (lastZone != null)
            {
                Collider2D zoneCollider = lastZone.GetComponent<Collider2D>();
                if (zoneCollider != null)
                {
                    targetPos = zoneCollider.ClosestPoint(transform.position);
                    targetPos.z = transform.position.z;
                }
            }

            returnCoroutine = StartCoroutine(SmoothReturnRoutine(targetPos, lastZone));
        }
    }

    private System.Collections.IEnumerator SmoothReturnRoutine(Vector3 targetPos, DropZone targetZone)
    {
        //부드럽게 이동+도착 후 요리 재개
        Vector3 startPos = transform.position;
        float elapsed = 0f;
        float duration = 0.2f;

        while (elapsed < duration)
        {
            if (isHandHeld) yield break;

            elapsed += Time.deltaTime;
            //Smoothstep을 쓰면 시작.끝이 부드러워짐
            float t = Mathf.SmoothStep(0, 1, elapsed / duration);
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        transform.position = targetPos;
        if (myRenderer != null) myRenderer.sortingOrder = 100;
    }

    public void InitializeData(List<Ingredients> ingredientsList)
    {
        foreach (var ing in ingredientsList)
        {
            AddDataToDict(ing.myType, ing.myState);

            if (ing.myType == IngredientType.Bread)
            {
                if (ing.hasKetchup)
                    AddDataToDict(IngredientType.Ketchup, IngredientState.None);

                if (ing.hasMustard)
                    AddDataToDict(IngredientType.Mustard, IngredientState.None);
            }
        }
        //PrintToastInfo();
    }

    private void AddDataToDict(IngredientType type, IngredientState state)
    {
        var key = (type, state);
        if (toastData.ContainsKey(key))
        {
            toastData[key]++;
        }
        else
        {
            toastData[key] = 1;
        }
    }

    private void PrintToastInfo()
    {
        Debug.Log("토스트 내용물");
        foreach (var kvp in toastData)
        {
            Debug.Log($"[{kvp.Key.Item1}], state:{kvp.Key.Item2} {kvp.Value}개");
        }
        Debug.Log("-----------------------");
    }

}
