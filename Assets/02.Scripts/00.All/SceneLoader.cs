using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;
    //static으로 클래스 자체에 소속
    //타 cs에서 Instance 붙여 호출

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void LoadSceneByName(string sceneName)
    {
        if (!IsValidSceneName(sceneName))
        {
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogWarning($"[GameSceneManager] LoadSceneByName 실패: Build Settings에 없는 씬입니다. sceneName={sceneName}");
            //Unity File-Build Setting
            return;
        }

        SceneManager.LoadScene(sceneName);
        AudioListener.pause = false;
    }

    /// <summary>
    /// Unity File-Build Profiles에서 sceneName이 비어 있지 않은지 검사한다.
    /// </summary>
    private bool IsValidSceneName(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogWarning("[GameSceneManager] sceneName이 비어 있습니다.");
            return false;
        }

        return true;
    }
}

