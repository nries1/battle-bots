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
                Debug.Log("ANIMATION SHOULD GO HERE");
                break;
            case PowerUpName.Blades:
                Debug.Log("ANIMATION SHOULD GO HERE");
                break;
            case PowerUpName.CattleCatcher:
                Debug.Log("ANIMATION SHOULD GO HERE");
                break;
            case PowerUpName.TeslaCannon:
                Debug.Log("ANIMATION SHOULD GO HERE");
                break;
            case PowerUpName.Hammer:
                Debug.Log("ANIMATION SHOULD GO HERE");
                break;
        }
    }

}