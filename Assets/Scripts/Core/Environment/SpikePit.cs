using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikePit : MonoBehaviour
{
    [SerializeField] float speed = 10.0f;

    private Vector3 startPosition;
    private MeshCollider meshCollider;
    private float targetY;
    private Vector3 target;
    private bool isAscending = true;
    private bool isPaused = false;
    [SerializeField] public int damageDealt = 20;

    void Start()
    {
        // Position the Spike Pit
        startPosition = transform.position;

        meshCollider = GetComponent<MeshCollider>();

        targetY = meshCollider.bounds.size.y + transform.position.y;

        target = new Vector3(transform.position.x, targetY, transform.position.z);

        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);



    }
    private void Update()
    {
        if (!isPaused)
        {
            if (isAscending)
            {
                transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, startPosition, speed * Time.deltaTime);
            }

            if (transform.position.y >= targetY)
            {
                isAscending = false;

            }
            else if (transform.position.y <= startPosition.y)
            {
                isAscending = true;
                StartCoroutine(PauseAtBottom());
            }
        }
    }

    private IEnumerator PauseAtBottom()
    {
        isPaused = true;
        float pauseDuration = Random.Range(1f, 10f);
        yield return new WaitForSeconds(pauseDuration);
        isPaused = false;
    }
}



