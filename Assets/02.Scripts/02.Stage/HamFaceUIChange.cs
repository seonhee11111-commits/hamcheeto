using UnityEngine;

public class HamFaceUIChange : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void TriggerSuccess()
    {
        if (anim != null) anim.SetTrigger("OrderSuccess");
    }

    public void TriggerFail()
    {
        if (anim != null) anim.SetTrigger("OrderFail");
    }
}
