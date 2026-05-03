using UnityEngine;

public class Zone_WasteCan : DropZone
{
    public override bool CanAccept(IngredientType type, IngredientState state)
    {
        //조건에 따라 CanAccept bool을 true/false로 반환.
        return true;
    }

    public override void DropAction(Ingredients ingredient)
    {
        SoundManager.Instance.PlaySFX(SFXType.Trash, 0.2f);
        Destroy(ingredient.gameObject);
    }
}
