using UnityEngine;
using UnityEngine.EventSystems;

public class ClickableObject : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{

    //[SerializeField] private SpriteRenderer spriteRenderer;
    //[SerializeField] private Sprite defaultSprite;
    //[SerializeField] private Sprite hoverOnSprite;

    private void Awake()
    {
        //spriteRenderer = GetComponent<SpriteRenderer>();
    }


    public virtual void OnPointerClick(PointerEventData eventData)
    {
        //클릭 시 원하는 Interaction 발생
        //이 ClickableObject를 부모 클래스로 삼음
        //자식 클래스를 만들어 실질 오브젝트에 부착
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {

    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {

    }


}
