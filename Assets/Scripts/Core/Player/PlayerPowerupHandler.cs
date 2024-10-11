using Unity.Netcode;
using UnityEngine;

public class PlayerPowerupHandler : NetworkBehaviour
{

    [Header("References")]
    [SerializeField] private GameObject sawBlades;
    [SerializeField] private GameObject blades;
    [SerializeField] private GameObject cattleCatcher;
    [SerializeField] private GameObject teslaCannon;
    [SerializeField] private GameObject hammer;


    public void HandlePowerUpCollision(PowerUpName powerUpName)
    {
        // if (!other.gameObject.CompareTag("PowerUp")) return;
        // string powerUpName = other.GetComponent<PowerUpController>().name;
        Debug.Log("Player receiving " + powerUpName);
        switch (powerUpName)
        {
            case PowerUpName.Saws:
                Debug.Log("Received " + powerUpName);
                sawBlades.SetActive(true);
                break;
            case PowerUpName.Blades:
                Debug.Log("Received " + powerUpName);
                blades.SetActive(true);
                break;
            case PowerUpName.CattleCatcher:
                Debug.Log("Received " + powerUpName);
                cattleCatcher.SetActive(true);
                break;
            case PowerUpName.TeslaCannon:
                Debug.Log("Received " + powerUpName);
                teslaCannon.SetActive(true);
                break;
            case PowerUpName.Hammer:
                Debug.Log("Received " + powerUpName);
                hammer.SetActive(true);
                break;
        }
    }

}