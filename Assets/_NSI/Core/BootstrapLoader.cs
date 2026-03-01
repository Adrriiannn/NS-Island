using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class BootstrapLoader : MonoBehaviour
{
    private const string FoundationSceneName = "01_TestIsland_Foundation";

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == FoundationSceneName)
        {
            return;
        }

        Debug.Log("Bootstrap -> Loading Foundation");
        SceneManager.LoadScene(FoundationSceneName);
    }
}
