using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatCollision : MonoBehaviour
{
    public bool collected_treasure = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "comoara")
        {
            collected_treasure = true;
        }
    }
}
