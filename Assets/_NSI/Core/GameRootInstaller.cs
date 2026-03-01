using UnityEngine;

public sealed class GameRootInstaller : MonoBehaviour
{
    [SerializeField] private GameObject gameRootPrefab;

    private bool _initialized;

    private void Awake()
    {
        if (_initialized)
        {
            return;
        }

        _initialized = true;

        GameObject existingGameRoot = GameObject.Find("GameRoot");
        if (existingGameRoot != null)
        {
            DontDestroyOnLoad(existingGameRoot);
            return;
        }

        if (gameRootPrefab == null)
        {
            return;
        }

        GameObject instance = Instantiate(gameRootPrefab);
        instance.name = "GameRoot";
        DontDestroyOnLoad(instance);
    }
}
