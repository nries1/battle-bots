using UnityEngine;

public class PlayerPowerupHandler : MonoBehaviour {

    [Header("References")]
    [SerializeField] private GameObject sawBlades;
    [SerializeField] private GameObject blades;
    [SerializeField] private GameObject cattleCatcher;
   
    
    public void HandlePowerUpCollision(string powerUpName) {
        // if (!other.gameObject.CompareTag("PowerUp")) return;
        // string powerUpName = other.GetComponent<PowerUpController>().name;
        Debug.Log("Player receiving " + powerUpName);
        switch(powerUpName) {
            case "Saws":
                Debug.Log("Received Saw");
                HandleSawPowerUp();
                break;
            case "Blades":
                Debug.Log("Received Blades");
                blades.SetActive(true);
                break;
            case "CattleCatcher":
                Debug.Log("Received CattleCatcher");
                cattleCatcher.SetActive(true);
                break;
        }
    }

    private void HandleSawPowerUp() {
        Debug.Log("Setting SAWS ACTIVE");
        sawBlades.SetActive(true);
    }
   
}