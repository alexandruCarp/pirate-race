using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureCollide : MonoBehaviour
{
    public bool collided = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        collided = true;
    }
}
