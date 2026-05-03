using UnityEngine;

public class SoundBridge : MonoBehaviour
{
    public void PlaySFX(string typeName)
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFXByName(typeName); 
        }
    }
}
