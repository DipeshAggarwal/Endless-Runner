using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChangeController : MonoBehaviour
{
    private SpriteRenderer sr;
    private BoxCollider2D cd;

    [SerializeField] private Color standingColor;
    [SerializeField] private Color visitedColor;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponentInParent<SpriteRenderer>();
        cd = GetComponent<BoxCollider2D>();

        // Make sure that the collider is just bigger than the Sprite.
        cd.size = new Vector2(sr.size.x, sr.size.y + 0.01f);
        cd.offset = new Vector2(0, 0);
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Color to set to when Player is on the platform.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            sr.color = standingColor;
        }
    }

    // Color to set to when Player leaves the platform.
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            sr.color = visitedColor;
        }
    }
}
