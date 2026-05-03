using UnityEngine;

public class Zone_Container_Ham : DropZone
{
    public override bool CanAccept(IngredientType type, IngredientState state)
    {
        //조건에 따라 CanAccept bool을 true/false로 반환.
        if (type == IngredientType.Ham && state == IngredientState.Raw) return true;
        else return false;
    }

    public override void DropAction(Ingredients ingredient)
    {
        //빵 들어오면 없어지게 (다시 넣는 척)
        Destroy(ingredient.gameObject);
    }
}
