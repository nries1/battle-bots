using UnityEngine;

public class HostSingleton : MonoBehaviour
{
    public HostGameManager GameManager { get; private set; }
    private static HostSingleton instance;
    public static HostSingleton Instance
    {
        get
        {
            instance = instance ? instance : FindObjectOfType<HostSingleton>();
            if (instance == null)
            {
                Debug.LogError("No HostSingleton in the scene");
            }
            return instance;
        }
    }
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    public void CreateHost()
    {
        GameManager = new HostGameManager();
    }
    private void OnDestroy()
    {
        GameManager?.Dispose();
    }
}
