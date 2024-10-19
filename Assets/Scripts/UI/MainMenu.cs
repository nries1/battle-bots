using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField joinCodeField;
    [SerializeField] private TMP_Text findMatchButtonText;
    [SerializeField] private TMP_Text queueStatus;
    [SerializeField] private TMP_Text queueTime;
    private bool isMatchmaking = false;
    private bool isCancelling = false;
    private void Start()
    {
        if (ClientSingleton.Instance == null) return;
        if (!queueStatus) return;
        queueStatus.text = string.Empty;
        queueTime.text = string.Empty;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
    public async void StartHost()
    {
        await HostSingleton.Instance.GameManager.StartHostAsync();
    }
    public async void StartClient()
    {
        await ClientSingleton.Instance.GameManager.StartClientAsync(joinCodeField.text);
    }
    public async void FindMatch()
    {
        // Start Queue
        ClientSingleton.Instance.GameManager.MatchMakeAsync(OnMatchMade);
        // Change start button text to cancel
        ToggleMatchmaking();
    }
    private void OnMatchMade(MatchmakerPollingResult result)
    {
        switch (result)
        {
            case MatchmakerPollingResult.Success:
                queueStatus.text = "Connecting...";
                break;
            case MatchmakerPollingResult.TicketCreationError:
                queueStatus.text = "TicketCreationError";
                break;
            case MatchmakerPollingResult.TicketCancellationError:
                queueStatus.text = "TicketCancellationError";
                break;
            case MatchmakerPollingResult.TicketRetrievalError:
                queueStatus.text = "TicketRetrievalError";
                break;
            case MatchmakerPollingResult.MatchAssignmentError:
                queueStatus.text = "MatchAssignmentError";
                break;
            default:
                queueStatus.text = "Unknown Error";
                break;
        }
    }
    private async void ToggleMatchmaking()
    {
        if (isCancelling) return;
        if (isMatchmaking)
        {
            // Cancel matchmaking
            queueStatus.text = "Cancelling...";
            isCancelling = true;
            await ClientSingleton.Instance.GameManager.CancelMatchMaking();
            isMatchmaking = false;
            isCancelling = false;
            findMatchButtonText.text = "Find Match";
            queueStatus.text = string.Empty;
        }
        else
        {
            findMatchButtonText.text = "Cancel";
            queueStatus.text = "Searching...";
            isMatchmaking = true;
            isCancelling = false;
        }
    }
}
