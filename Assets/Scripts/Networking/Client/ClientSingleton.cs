using System.Threading.Tasks;
using UnityEngine;

public class ClientSingleton : MonoBehaviour
{
    private ClientGameManager gameManager;
    private static ClientSingleton instance;
    public static ClientSingleton Instance
    {
        get
        {
            instance = instance ? instance : FindObjectOfType<ClientSingleton>();
            if (instance == null)
            {
                Debug.LogError("No ClientSingleton in the scene");
            }
            return instance;
        }
    }
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    public async Task CreateClient()
    {
        gameManager = new ClientGameManager();
        await gameManager.InitAsync();
    }
}
