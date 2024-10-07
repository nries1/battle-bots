using Unity.Netcode;
using UnityEngine;

public class PlayerPowerupHandler : NetworkBehaviour {

    [Header("References")]
    [SerializeField] private GameObject sawBlades;
    [SerializeField] private GameObject blades;
    [SerializeField] private GameObject cattleCatcher;
    [SerializeField] private GameObject teslaCannon;
    [SerializeField] private GameObject hammer;
   
    
    public void HandlePowerUpCollision(string powerUpName) {
        // if (!other.gameObject.CompareTag("PowerUp")) return;
        // string powerUpName = other.GetComponent<PowerUpController>().name;
        Debug.Log("Player receiving " + powerUpName);
        switch(powerUpName) {
            case "Saws":
                Debug.Log("Received " + powerUpName);
                HandleSawPowerUp();
                break;
            case "Blades":
                Debug.Log("Received " + powerUpName);
                blades.SetActive(true);
                break;
            case "CattleCatcher":
                Debug.Log("Received " + powerUpName);
                cattleCatcher.SetActive(true);
                break;
            case "TeslaCannon":
                Debug.Log("Received " + powerUpName);
                teslaCannon.SetActive(true);
                break;
            case "Hammer":
                Debug.Log("Received " + powerUpName);
                hammer.SetActive(true);
                break;
        }
    }

    private void HandleSawPowerUp() {
        Debug.Log("Setting SAWS ACTIVE");
        sawBlades.SetActive(true);
    }
   
}