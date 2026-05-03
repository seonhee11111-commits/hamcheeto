using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    //전역 접근을 위하여, 싱글톤 스크립트로 만듦

    public static CursorManager instance { get; private set; }

    [Header("Cursor Settings")]
    [SerializeField] private Image uiCursorImage;
    [SerializeField] private Sprite handCursorSprite;
    [SerializeField] private Sprite handHoldCursorSprite;


    //전역 읽기 가능. 햄손이 무언가를 들고 있니?
    public bool IsHandHolding { get; set; }

    private void Awake()
    {
        if (instance == null) 
        { instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);

        Cursor.visible = false;
        //SetCursorState(false); // 초기 커서 세팅
    }

    private void Update()
    {
        if (uiCursorImage != null)
        {
            //매 프레임마다 UI 이미지를 마우스 위치로 이동
            Vector2 mousePos = Mouse.current.position.ReadValue();
            uiCursorImage.transform.position = mousePos;

            bool isPressed = Mouse.current.leftButton.isPressed;
            uiCursorImage.sprite=isPressed? handHoldCursorSprite : handCursorSprite;
        }
    }

    //public void SetCursorState(bool isHandHolding)
    //{
    //    //객체를 집고 내려놓을 때마다 호출

    //    IsHandHolding = isHandHolding;

    //    uiCursorImage.sprite = isHandHolding ? handHoldCursorSprite : handCursorSprite;
    //}

}
