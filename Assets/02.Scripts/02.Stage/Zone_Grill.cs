using UnityEngine;
using System.Collections.Generic;

public class Zone_Grill : DropZone
{
    private List<Ingredients> cookingList = new List<Ingredients>();

    [SerializeField] private AudioSource grillAudio;

    public override bool CanAccept(IngredientType type, IngredientState state)
    {
        if (type == IngredientType.Bread) return true;
        if (type == IngredientType.Ham) return true;
        if (type == IngredientType.Cheese) return true;
        if (type == IngredientType.Egg) return true;
        if (type == IngredientType.Cabbage) return true;
        if (type == IngredientType.Onion) return true;
        if (type == IngredientType.Pickle) return true;
        return false;
    }

    public override void DropAction(Ingredients ingredient)
    {
        ingredient.currentZone = this;

        if (ingredient.myType==IngredientType.Egg && ingredient.myState==IngredientState.EggShell)
        { ingredient.ChangeState(IngredientState.Raw); }

        if (!cookingList.Contains(ingredient))
        {
            cookingList.Add(ingredient);

            if (cookingList.Count == 1 && grillAudio != null && grillAudio.isPlaying == false)
            { grillAudio.Play(); }
        }

        ingredient.StartCookingEffect();
    }

    private void Update()
    {
        for (int i = cookingList.Count - 1; i >= 0; i--)
        {
            //역순 순회하며 진행시간 증가 처리
            Ingredients ing = cookingList[i];

            ing.currentCookTime += Time.deltaTime; //요리시간 축적

            if (ing.currentCookTime >= ing.targetBurnTime && ing.myState != IngredientState.Burnt)
            {
                ing.ChangeState(IngredientState.Burnt);
            }
            else if (ing.currentCookTime >= ing.targetCookTime && ing.myState == IngredientState.Raw)
            {
                ing.ChangeState(IngredientState.Cooked);
            }

            ing.UpdateIngredientGaugeUI();


            //***추후 재료가 50개, 100개... 이렇게 늘어나는 등 게임 사이즈가 커지면 Scriptable Object 사용을 고려할 것.
            //치즈 데이터, 햄 데이터 등 에셋을 만들어두고 여러 프리팹이 돌려서 사용
        }
    }

    public void RemoveFromGrill(Ingredients ingredient)
    {
        //재료를 집어들 경우 Grill 요리 중 리스트에서 제외
        if (cookingList.Contains(ingredient))
        {
            cookingList.Remove(ingredient);
            if(cookingList.Count==0 && grillAudio!=null && grillAudio.isPlaying)
            { grillAudio.Stop(); }
        }
    }
}


