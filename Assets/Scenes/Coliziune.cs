using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Barrel")
        {
            
            // Destroy the obstacle
            Destroy(collision.gameObject);
            GameObject[] boats = NewBehaviourScript.boats;
            List<GameObject> obstacles = NewBehaviourScript.obstacles;
            if (obstacles.Contains(collision.gameObject))
            {
                obstacles.Remove(collision.gameObject);
            }
            foreach (GameObject boat in boats)
            {
                if (collision.transform.position.y == boat.transform.position.y)
                {
                    boat.transform.position += new Vector3(-5f, 0, 0);
                    break;
                }
            }
        }
    }
}