using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class SauceBottle : ClickableObject
{
    [Header("Sauce Settings")]
    //[SerializeField] private GameObject sauceSplatPrefab;
    public IngredientType mySauceType;
    [SerializeField] private GameObject myShadow;
    //인스펙터 프리팹 연결 필요

    private bool isEquipped = false;
    private bool isSqueezing = false;
    private Vector3 originPos;
    private Quaternion originRot;
    private Camera mainCam;
    private Collider2D myCollider;
    private SpriteRenderer myRenderer;

    private void Start()
    {
        mainCam = Camera.main;
        originPos = transform.position;
        originRot = transform.rotation;
        myCollider = GetComponent<Collider2D>();
        myRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (isEquipped)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector3 worldPos = mainCam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, Mathf.Abs(mainCam.transform.position.z)));
            transform.position = worldPos;

            if (Mouse.current.leftButton.wasPressedThisFrame && !isSqueezing)
            {
                isSqueezing = true;
                //Todo: 애니메이터 트리거 호출
                transform.localScale = new Vector3(0.9f, 0.8f, 1f); //임시 스케일조정
            }

            if (Mouse.current.leftButton.wasReleasedThisFrame && isSqueezing)
            {
                isSqueezing = false;
                TrySqueezeSauce();
                ReturnToOrigin();
            }
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        //빈 손일 때 소스통 equip
        if (!isEquipped && !CursorManager.instance.IsHandHolding)
        {
            //Debug.Log("EquipBottle();");
            EquipBottle();
        }
    }

    private void EquipBottle()
    {
        isEquipped = true;
        //CursorManager.instance.SetCursorState(true);
        CursorManager.instance.IsHandHolding = true;
        myRenderer.sortingOrder = 998;

        if (myShadow!=null) myShadow.gameObject.SetActive(false);

        transform.rotation = Quaternion.Euler(0, 0, 165f);
    }

    private void TrySqueezeSauce()
    {
        if (myCollider != null)
        {
            myCollider.enabled = false;
        }

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.zero);
        //Debug.Log(hit.collider);
        if (hit.collider != null)
        {
            Zone_Package packageZone = hit.collider.GetComponent<Zone_Package>();

            if (packageZone != null)
            {
                Ingredients topIng = packageZone.GetTopIngredient();

                //맨 위가 빵이면 소스바르기
                if (topIng != null && topIng.myType == IngredientType.Bread)
                {
                    topIng.ApplySauce(mySauceType);
                    SoundManager.Instance.PlaySFX(SFXType.SauceSqueeze, 0.2f);
                    //Debug.Log("소스짜기ok");
                    return;
                }
            }

        }
        //Debug.Log("소스짜기실패");
    }

    private void ReturnToOrigin()
    {
        isEquipped = false;
        //CursorManager.instance.SetCursorState(false);
        CursorManager.instance.IsHandHolding = false;
        if (myRenderer != null) myRenderer.sortingOrder = 100;

        myCollider.enabled = true;
        if (myShadow != null) myShadow.gameObject.SetActive(true);

        transform.position=originPos;
        transform.rotation=originRot;
        transform.localScale = Vector3.one;
    }

}
