using UnityEngine;

public class SceneLoadBridge : MonoBehaviour
{
    public void GoToScene(string sceneName)
    {
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadSceneByName(sceneName);
        }
        else
        {
            Debug.LogError("SceneLoader Null");
        }
    }

}
