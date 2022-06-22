using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    [SerializeField] private int chanceToSpawn;

    private Player player;

    // Start is called before the first frame update
    void Start()
    {
        // A random roll to decide whether to keep the Trap or destroy it.
        if (Random.Range(1, 100) > chanceToSpawn)
        {
            Destroy(this.gameObject);
        }

        // Get reference to the Player script.
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (player.moveSpeed > player.defaultMoveSpeed)
            {
                 player.knockback();
            }
            else
            {
                GameManager.instance.gameEnds();
            }
        }
    }
}
