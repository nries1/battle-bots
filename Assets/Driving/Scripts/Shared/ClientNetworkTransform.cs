using Unity.Netcode.Components;
using UnityEngine;

public enum AuthorityMode
{
    Server,
    Client
}
// Allow the clients to control movement to avoid latency, but maintain the server as the final authority on player position
// Steps for Client - Server communication are as follows
/*
    1. Client captures input from player
    2. Client sends input payload to the server
    3. Client records player state
    3. [Simultaneous] Client moves the player to their desired location
    3. [Simultaneous] Server Captures the requested client movement
    4. Server sends state back to the client
    5. Client reconciles server state with its own state
*/
[DisallowMultipleComponent]
public class ClientNetworkTransform : NetworkTransform
{
    public AuthorityMode authorityMode = AuthorityMode.Client;

    protected override bool OnIsServerAuthoritative() => authorityMode == AuthorityMode.Server;
}