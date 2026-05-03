using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class IngredientsContainer : ClickableObject
{
    //---------------------------------레퍼런스

    [Header("Ingredients Prefab")]
    [SerializeField] private GameObject ingredientsPrefab;

    private Camera mainCam;

    //---------------------------------함수
    private void Start()
    {
        mainCam = Camera.main;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (ingredientsPrefab == null || CursorManager.instance.IsHandHolding) return;
        SpawnIngredients();
        //CursorManager.instance.SetCursorState(true);
        CursorManager.instance.IsHandHolding = true;
    }

    private void SpawnIngredients()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        //현재 마우스 위치를 읽어옴
        Vector3 screenPosWithZ = new Vector3(mouseScreenPos.x, mouseScreenPos.y, Mathf.Abs(mainCam.transform.position.z));
        //z값이 0(카메라 위치)로 처리됨. z값을 카메라가 떨어진 만큼 더해주는 방식으로 세팅하는 코드
        Vector3 spawnPos = mainCam.ScreenToWorldPoint(screenPosWithZ);
        //현재 마우스포인터 위치를 스폰 위치로 지정 (z값도 세팅해야 하므로 Vector3)

        GameObject spawnedObj = Instantiate(ingredientsPrefab, spawnPos, Quaternion.identity);
        //spawnedObj라는 GameObject 생성(원본 프리팹, 생성될 위치, 회전값) ---> Quaternion.identity = 회전 없음(기본)

        spawnedObj.GetComponent<Ingredients>().PickUpFromContainer();
        //생성된 오브젝트의 컴포넌트를 가져온 다음, PickUp 함수 실행(들고 있는 상태로 고정)

    }

}
