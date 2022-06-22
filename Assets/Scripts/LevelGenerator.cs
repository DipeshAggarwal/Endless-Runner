using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public Transform[] levelParts;
    private GameObject player;

    public Vector3 nextPartPosition;

    public float partDrawDistance;
    public float partDeleteDistance;



    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        DeletePart();
        GeneratePart();
    }

    private void GeneratePart()
    {
        while ((nextPartPosition.x - player.transform.position.x) < partDrawDistance)
        {
            Transform part = levelParts[Random.Range(0, levelParts.Length)];
            Transform newPart = Instantiate(part, nextPartPosition - part.Find("Start point").position, transform.rotation, transform);

            nextPartPosition = newPart.Find("End point").position;
        }
    }

    private void DeletePart()
    {
        if (transform.childCount > 0)
        {
            Transform part = transform.GetChild(0);

            Vector3 distance = player.transform.position - part.position;

            if (distance.x > partDeleteDistance)
            {
                Destroy(part.gameObject);
            }
        }
    }
}
