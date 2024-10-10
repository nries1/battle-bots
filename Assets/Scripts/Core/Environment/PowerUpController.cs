using UnityEngine;

public class PowerUpController : MonoBehaviour
{
    [SerializeField] private string powerUpName;
    private bool ascending = true;
    private Vector3 target;
    private Vector3 startPos;
    void Start()
    {
        target = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        startPos = transform.position;
    }
    void LateUpdate()
    {
        // rotate around the y axis
        transform.RotateAround(transform.position, Vector3.up, 40 * Time.deltaTime);
        // bob up and down
        if (ascending)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, startPos, Time.deltaTime);
        }
        if (transform.position.y >= target.y)
        {
            ascending = false;
        }
        else if (transform.position.y <= startPos.y)
        {
            ascending = true;
        }
    }
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided From Powerup");
        PlayerPowerupHandler player = other.gameObject.GetComponent<PlayerPowerupHandler>();
        if (player == null) return;
        player.HandlePowerUpCollision(powerUpName);
    }
}