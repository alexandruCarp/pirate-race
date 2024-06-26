using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatCollision : MonoBehaviour
{
    public bool collected_treasure = false;
    public bool should_slow_down = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "comoara")
        {
            collected_treasure = true;
        }

        if (collision.gameObject.tag == "Barrel")
        {
            
            // Destroy the obstacle
            Destroy(collision.gameObject);
            List<GameObject> obstacles = NewBehaviourScript.obstacles;
            if (obstacles.Contains(collision.gameObject))
            {
                obstacles.Remove(collision.gameObject);
            }
            should_slow_down = true;
        }
    }
}
