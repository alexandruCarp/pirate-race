using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlBarca : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // fa barca sa se miste daca apas sageata
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position += new Vector3(-0.1f, 0, 0);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.position += new Vector3(0.1f, 0, 0);
        }
    }
}
