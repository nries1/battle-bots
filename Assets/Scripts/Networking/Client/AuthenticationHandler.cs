using System.Threading.Tasks;
using Unity.Services.Authentication;

public static class AuthenticationHandler
{
    public static AuthState AuthState { get; private set; } = AuthState.NotAuthenticated;
    public static async Task<AuthState> DoAuth(int maxTries = 5)
    {
        if (AuthState == AuthState.Authenticated) return AuthState;
        AuthState = AuthState.Authenticating;
        int tries = 0;
        while (tries < maxTries && AuthState == AuthState.Authenticating)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
            {
                AuthState = AuthState.Authenticated;
                break;
            }
            tries++;
            await Task.Delay(1000);
        }
        return AuthState;
    }
}

public enum AuthState
{
    NotAuthenticated,
    Authenticating,
    Authenticated,
    Error,
    TimeOut
}