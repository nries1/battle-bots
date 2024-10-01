using TMPro;
using Unity.Collections;
using Unity.Netcode;

public class PlayerHud : NetworkBehaviour
{
    private NetworkVariable<NetworkString> playersName = new NetworkVariable<NetworkString>();
    private bool overlaySet = false;
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            playersName.Value = $"Player {OwnerClientId}";
        }
    }
    public void SerOverlay()
    {
        var localPlayerOverlay = gameObject.GetComponentInChildren<TextMeshPro>();
        localPlayerOverlay.text = playersName.Value;
    }
    private void Update()
    {
        if (!overlaySet && !string.IsNullOrEmpty(playersName.Value))
        {
            SerOverlay();
            overlaySet = true;
        }
    }
}

