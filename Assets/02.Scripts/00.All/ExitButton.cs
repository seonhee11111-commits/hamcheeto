using UnityEngine;

public class ExitButton : MonoBehaviour
{
    public void ExitGame()
    {
        /*if (UnityEditor.EditorApplication.isPlaying)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
        else*/
        Application.Quit();
    }

}
