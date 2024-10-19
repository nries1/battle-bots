using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Core;
using UnityEngine;

public class ServerSingleton : MonoBehaviour
{
    public ServerGameManager GameManager { get; private set; }
    private static ServerSingleton instance;
    public static ServerSingleton Instance
    {
        get
        {
            instance = instance ? instance : FindObjectOfType<ServerSingleton>();
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
    public async Task CreateServer()
    {
        await UnityServices.InitializeAsync();
        GameManager = new ServerGameManager(
            ApplicationData.IP(),
            ApplicationData.Port(),
            ApplicationData.QPort(),
            NetworkManager.Singleton
        );
    }
    private void OnDestroy()
    {
        GameManager?.Dispose();
    }
}
