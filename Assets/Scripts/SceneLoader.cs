using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(string sceneName="")
    {
        if (sceneName != "")
            SceneManager.LoadScene(sceneName);
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}