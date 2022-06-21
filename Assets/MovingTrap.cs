using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTrap : MonoBehaviour
{
    [SerializeField] private Transform[] movePoints;
    [SerializeField] private int nextPosition;
    [SerializeField] private float trapSpeed;
    [SerializeField] private float rotationMultiplier;
    [SerializeField] private int chanceToSpawn;

    private Player player;

    // Start is called before the first frame update
    void Start()
    {
        // A random roll to decide whether to keep the Trap or destroy it.
        if (Random.Range(1, 100) > chanceToSpawn)
        {
            // Since this script is attached to a child element, make sure to destroy the player to remove the object.
            Destroy(transform.parent.gameObject);
        }

        // Get reference to the Player script.
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        // Move the object towards the next selected point.
        transform.position = Vector3.MoveTowards(transform.position, movePoints[nextPosition].position, trapSpeed * Time.deltaTime);

        // If distance between the object and the point is less then 0.2, move to next point.
        if (Vector3.Distance(transform.position, movePoints[nextPosition].position) < 0.2f)
        {
            nextPosition++;
        }

        // If next point is more than total points, reset to zero.
        if (nextPosition >= movePoints.Length)
        {
            nextPosition = 0;
        }

        // Control the rotation of the object based on the direction of travel.
        if (transform.position.x > movePoints[nextPosition].position.x)
        {
            transform.Rotate(new Vector3(0, 0, 100 * rotationMultiplier) * Time.deltaTime);
        }
        else
        {
            transform.Rotate(new Vector3(0, 0, 100 * -rotationMultiplier) * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            player.knockback();
        }
    }
}
