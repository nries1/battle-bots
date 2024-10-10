using System.Threading.Tasks;
using Unity.Services.Core;
using UnityEngine.SceneManagement;

public class ClientGameManager
{
    private const string menuSceneName = "menu";
    public async Task<bool> InitAsync()
    {
        await UnityServices.InitializeAsync();
        AuthState authState = await AuthenticationHandler.DoAuth(5);
        return authState == AuthState.Authenticated;
    }
    public void GoToMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }
}
