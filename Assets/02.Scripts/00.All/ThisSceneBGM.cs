using UnityEngine;

public class ThisSceneBGM : MonoBehaviour
{
    [SerializeField] private AudioClip SceneBGM;
    [SerializeField] private float volume = 0.2f;

    private void Start()
    {
        if (SoundManager.Instance != null)
        { SoundManager.Instance.PlayBGM(SceneBGM, volume); }
    }

}
