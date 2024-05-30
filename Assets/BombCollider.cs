using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombCollider : MonoBehaviour
{
    public bool collided = false;
private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Barrel")
        {
            
            // Destroy the obstacle
            Destroy(collision.gameObject);
            List<GameObject> obstacles = NewBehaviourScript.obstacles;
            if (obstacles.Contains(collision.gameObject))
            {
                obstacles.Remove(collision.gameObject);
            }
            collided = true;
        }
    }
}
