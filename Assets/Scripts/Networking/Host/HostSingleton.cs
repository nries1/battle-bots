using System.Threading.Tasks;
using UnityEngine;

public class HostSingleton : MonoBehaviour
{
    private HostGameManager gameManager;
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
        gameManager = new HostGameManager();
    }
}
