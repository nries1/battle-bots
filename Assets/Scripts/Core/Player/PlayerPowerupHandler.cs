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
        switch (powerUpName)
        {
            case PowerUpName.Saws:
                sawBlades.SetActive(true);
                break;
            case PowerUpName.Blades:
                blades.SetActive(true);
                break;
            case PowerUpName.CattleCatcher:
                cattleCatcher.SetActive(true);
                break;
            case PowerUpName.TeslaCannon:
                teslaCannon.SetActive(true);
                break;
            case PowerUpName.Hammer:
                hammer.SetActive(true);
                break;
        }
    }

}