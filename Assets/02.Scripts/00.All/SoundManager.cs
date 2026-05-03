using UnityEngine;
using System.Collections.Generic;

public enum SFXType
{
    PackToast,
    SauceSqueeze,
    ButtonPop,
    MeowHello,
    MeowHappy,
    MeowAngry,
    ToastSuccess,
    ToastFail,
    Clear,
    GameOver,
    Trash,
    GrillOn,
    GrillStateChange
}

[System.Serializable]
public struct SFXData
{
    public SFXType sfxType;
    public AudioClip clip;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("SoundSource")]
    [SerializeField] private AudioSource BGMSource;
    [SerializeField] private AudioSource SFXSource;

    [Header("SFX List")]
    [SerializeField] private List<SFXData> sfxList = new List<SFXData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SFXSource.ignoreListenerPause = true;
        }
        else Destroy(gameObject);
    }

    public void PlayBGM(AudioClip bgmClip, float volume = 0.15f, bool loop=true)
    {
        if (bgmClip == null || BGMSource==null)
        { Debug.Log("bgmClip || BGMSource null");  return; }

        BGMSource.clip = bgmClip;
        BGMSource.loop = loop;
        BGMSource.volume = Mathf.Clamp01(volume);
        BGMSource.Play();
    }

    public void PlaySFX(SFXType type, float volume = 0.4f)
    {
        if (SFXSource == null) return;

        foreach (var sfx in sfxList)
        {
            if (sfx.sfxType == type)
            {
                SFXSource.PlayOneShot(sfx.clip, volume);
                return;
            }
        }
        Debug.Log($"{type}SFX Null");
    }

    public void PlaySFXByName(string sfxName)
    {
        if (System.Enum.TryParse(sfxName, out SFXType type))
        {
            PlaySFX(type);
        }
        else Debug.Log($"{sfxName} SFX Null");
    }

    /*
    public void PauseBGM()
    {
        if (BGMSource != null && BGMSource.isPlaying)
        {
            BGMSource.Pause();
        }
    }

    public void ResumeBGM()
    {
        if (BGMSource != null && !BGMSource.isPlaying)
        {
            BGMSource.UnPause();
        }
    }
    */

}
