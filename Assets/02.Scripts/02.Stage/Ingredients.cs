using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[System.Serializable]
public struct BreadSpriteSet
{
    public IngredientState state;
    public Sprite plain;
    public Sprite ketchup;
    public Sprite mustard;
    public Sprite both;
}


[System.Serializable]
public struct StateSpritePair
{
    public IngredientState state;
    public Sprite sprite;
}


public class Ingredients : ClickableObject
{
    private bool isHandHeld = false;
    private Camera mainCam;
    private Collider2D myCollider;
    private SpriteRenderer myRenderer;

    [Header("Ingredient Data")]
    public IngredientType myType;
    public IngredientState myState;
    //인스펙터 연결 필요
    public float currentCookTime = 0f;

    [Header("Cooking Settings")]
    public float targetCookTime = 3f;
    public float targetBurnTime = 6f;
    //인스펙터에서 프리팹마다 시간을 세팅 가능
    [SerializeField] private List<StateSpritePair> stateSprites;

    [Header("UI Settings")]
    public Image cookTimeGauge;

    [Header("Bread Settings")]
    public bool hasKetchup = false;
    public bool hasMustard = false;
    [SerializeField] private List<BreadSpriteSet> breadSpriteSets;

    private Vector3 lastPos;
    private DropZone lastZone;
    public DropZone currentZone;
    private Coroutine returnCoroutine;
    private bool isFromContainer = false;
    private Coroutine cookingEffectRoutine;



    private void Awake()
    {
        mainCam = Camera.main;
        myCollider = GetComponent<Collider2D>();
        myRenderer = GetComponent<SpriteRenderer>();
        UpdateIngredientSprite();
    }

    private void Update()
    {
        if (isHandHeld)
        {
            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
            Vector3 screenPosWithZ = new Vector3(mouseScreenPos.x, mouseScreenPos.y, Mathf.Abs(mainCam.transform.position.z));
            //z값이 0(카메라 위치)로 처리됨. z값을 카메라가 떨어진 만큼 더해주는 방식으로 세팅하는 코드
            transform.position = (Vector2)mainCam.ScreenToWorldPoint(screenPosWithZ);

            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                TryDrop();
            }
        }

    }

    public void UpdateIngredientGaugeUI()
    {
        if (cookTimeGauge == null) return;
        if (currentZone is Zone_Package ) 
        {
            cookTimeGauge.gameObject.SetActive(false);
            return;
        }
        if(currentCookTime <=0 || myState == IngredientState.Burnt)
        {
            cookTimeGauge.gameObject.SetActive(false);
            return;
        }

        cookTimeGauge.gameObject.SetActive(true);
        if (myState == IngredientState.Raw)
        {
            cookTimeGauge.fillAmount = currentCookTime / targetCookTime;
        }
        else if (myState == IngredientState.Cooked)
        {
            float burnDuration = targetBurnTime - targetCookTime;
            float currentBurnTime = currentCookTime - targetCookTime;
            cookTimeGauge.fillAmount = currentBurnTime / burnDuration;
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

    public void PickUpFromContainer()
    {
        isFromContainer = true;
        TryPickUp();
    }

    public void TryPickUp()
    {
        StopCookingEffect();

        isHandHeld = true;
        myRenderer.sortingOrder = 998;
        //CursorManager.instance.SetCursorState(true);
        CursorManager.instance.IsHandHolding = true;
        lastPos = transform.position;

        if (returnCoroutine != null)
        {   //날아가던 거 잡았을 때는 coroutine 중지
            StopCoroutine(returnCoroutine); }

        if (currentZone is Zone_Grill grill)
        {   grill.RemoveFromGrill(this);   }

        lastZone = currentZone;
        currentZone = null;

        if (myCollider != null)
        {
            //들어올리면서 현재 객체의 콜라이더를 잠깐 끔 (Zone 인식 방해 방지)
            myCollider.enabled = false;
        }
    }

    public void TryDrop()
    {
        isHandHeld = false;
        //CursorManager.instance.SetCursorState(false);
        CursorManager.instance.IsHandHolding = false;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.zero);
        if (myCollider != null) myCollider.enabled = true;

        if (hit.collider != null)
        {
            DropZone zone = hit.collider.GetComponent<DropZone>();
            if (zone != null && zone.CanAccept(myType, myState))
            {
                isFromContainer = false;
                //Debug.Log($"TryDrop: zone:{zone}");
                if(myRenderer!=null) myRenderer.sortingOrder = 100;
                zone.DropAction(this);
                return;
            }
        }

        DropFailure();
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

        if (targetZone != null && targetZone.CanAccept(myType, myState))
        {
            //도착하면 나를 등록해 DropAction 실행
            targetZone.DropAction(this);
        }
    }

    public void ChangeState(IngredientState newState)
    {
        if (myState != newState)
        {
            hasKetchup = false;
            hasMustard = false;
        }
        myState = newState;
        UpdateIngredientSprite();
        SoundManager.Instance.PlaySFX(SFXType.GrillStateChange,0.2f);
    }

    public void UpdateIngredientSprite()
    {
        if (myRenderer == null) return;

        if ((myType == IngredientType.Bread))
        {
            foreach (var set in breadSpriteSets)
            {
                if (set.state == myState)
                {
                    if (hasKetchup && hasMustard) myRenderer.sprite = set.both;
                    else if (hasKetchup) myRenderer.sprite = set.ketchup;
                    else if (hasMustard) myRenderer.sprite = set.mustard;
                    else myRenderer.sprite = set.plain;
                    return;
                }
            }
        }

        else
        {
            foreach (var pair in stateSprites)
            {
                if (pair.state == myState)
                {
                    myRenderer.sprite = pair.sprite;
                    return;
                }
            }
        }
    }

    public void ApplySauce(IngredientType sauceType)
    {
        if (myType != IngredientType.Bread) return;

        if (sauceType == IngredientType.Ketchup) hasKetchup = true;
        if (sauceType == IngredientType.Mustard) hasMustard = true;

        UpdateIngredientSprite();
    }

    public void StartCookingEffect()
    {
        if (cookingEffectRoutine == null)
        {
            cookingEffectRoutine = StartCoroutine(CookingEffectRoutine());
        }
    }

    public void StopCookingEffect()
    {
        if (cookingEffectRoutine != null)
        {
            StopCoroutine(cookingEffectRoutine);
            cookingEffectRoutine = null;
            transform.localScale = Vector3.one;
        }
    }

    private IEnumerator CookingEffectRoutine()
    {
        Vector3 initialScale = Vector3.one; //1,1,1

        while (true)
        {
            //Mathf.PingPong(시간,길이) 시간에 따라 0~길이 사이 왕복
            float pingPong = Mathf.PingPong(Time.time * 0.5f, 0.04f);
            //float warp = Mathf.Sin(Time.time * 20f) * 0.01f;
            transform.localScale = initialScale + new Vector3(pingPong, pingPong, 0);
            //transform.localScale = initialScale + new Vector3(warp, warp, 0);
            yield return null;
        }
    }

}
