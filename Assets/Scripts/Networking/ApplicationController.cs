using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ApplicationController : MonoBehaviour
{
    [SerializeField] private ClientSingleton clientPrefab;
    [SerializeField] private HostSingleton hostPrefab;
    [SerializeField] private ServerSingleton serverPrefab;
    private async void Start()
    {
        DontDestroyOnLoad(gameObject);
        // the dedicated server doesn't have a graphics device because it doesn't render anything because it doesn't have anyone playing on it
        await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
    }

    private async Task LaunchInMode(bool isDedicatedServer)
    {
        if (isDedicatedServer)
        {
            // Spawn in a server singleton
            ServerSingleton serverSingleton = Instantiate(serverPrefab);
            // connect to UGS
            await serverSingleton.CreateServer();

            await serverSingleton.GameManager.StartGameServerAsync();
        }
        else
        {
            HostSingleton hostSingleton = Instantiate(hostPrefab);
            hostSingleton.CreateHost();
            ClientSingleton clientSingleton = Instantiate(clientPrefab);
            bool authenticated = await clientSingleton.CreateClient();
            if (authenticated)
            {
                clientSingleton.GameManager.GoToMenu();
            }
        }
    }
}
