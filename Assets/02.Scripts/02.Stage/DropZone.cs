using UnityEngine;

public abstract class DropZone : MonoBehaviour
{

    public abstract bool CanAccept(IngredientType type, IngredientState state);

    public abstract void DropAction(Ingredients ingredient);

    //DropZone의 유형(영역): Grill, Package, Container, WasteCan, None
    //Grill -> 체류 시간에 따른 Ingredient State 변경
    //Package -> '빵'만 가능(생빵or구운빵) --> '빵'을 올리면 List 생성, 조리 공간과 별도의 영역으로 취급, 맨 윗빵을 덮은 후 (List의 첫 칸과 마지막 칸이 빵일 때 포장 가능)
    //Container -> 'raw'상태의 (익히지 않은) 재료만 가능 --> 일치하지 않는 다른 Container일 경우 Back 판정.
    //WasteCan -> 어떤 객체든 모두 Destroy 판정. Undo Stack에서도 없애버림. Sound 재생.
    //이상 Zone 시스템은 Ingredient에 한함. 조합체(포장완료 상태)의 경우 별도의 Click, Drag and Drop 시스템을 연결해서 따로 처리할 것.
}

