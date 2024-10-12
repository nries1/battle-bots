using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public static class AuthenticationHandler
{
    public static AuthState AuthState { get; private set; } = AuthState.NotAuthenticated;
    public static async Task<AuthState> DoAuth(int maxTries = 5)
    {
        if (AuthState == AuthState.Authenticated) return AuthState;
        if (AuthState == AuthState.Authenticating)
        {
            await Authenticating();
            return AuthState;
        }
        await SignInAnonymouslyAsync(maxTries);
        return AuthState;
    }

    private static async Task<AuthState> Authenticating()
    {
        while (AuthState == AuthState.Authenticating || AuthState == AuthState.NotAuthenticated)
        {
            await Task.Delay(200);
        }
        return AuthState;
    }

    private static async Task SignInAnonymouslyAsync(int maxTries)
    {
        AuthState = AuthState.Authenticating;
        int tries = 0;
        while (tries < maxTries && AuthState == AuthState.Authenticating)
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
                {
                    AuthState = AuthState.Authenticated;
                    break;
                }
            }
            catch (AuthenticationException exception)
            {
                Debug.LogError(exception);
                AuthState = AuthState.Error;
            }
            catch (RequestFailedException exception)
            {
                Debug.LogError(exception);
                AuthState = AuthState.Error;
            }
            tries++;
            await Task.Delay(1000);
        }
        if (AuthState != AuthState.Authenticated)
        {
            Debug.LogWarning($"Failed to sign player in after {tries} attempts");
            AuthState = AuthState.TimeOut;
        }
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