using Unity.Netcode.Components;
using UnityEngine;

public enum AuthorityMode
{
    Server,
    Client
}
// Allow the clients to control movement to avoid latency
[DisallowMultipleComponent]
public class ClientNetworkTransform : NetworkTransform
{
    public AuthorityMode authorityMode = AuthorityMode.Client;

    protected override bool OnIsServerAuthoritative() => authorityMode == AuthorityMode.Server;
}