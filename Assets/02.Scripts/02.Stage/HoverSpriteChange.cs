using UnityEngine;
using UnityEngine.EventSystems;

public class HoverSpriteChange : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite hoverOnSprite;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        //호버스프라이트가 존재하면 Enter 시에 전환
        if (hoverOnSprite != null)
        { spriteRenderer.sprite = hoverOnSprite; }
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        //디폴트스프라이트가 존재하면 Exit 시에 전환
        if (defaultSprite != null)
        { spriteRenderer.sprite = defaultSprite; }
    }

}
