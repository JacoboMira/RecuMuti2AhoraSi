using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackerPointer : MonoBehaviour
{
    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerJoseluis>().gameObject;
        transform.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            Destroy(gameObject);
        }
        else
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A))
            {
                transform.position = player.transform.position;

                if (Input.GetKey(KeyCode.W))
                {
                    transform.position += Camera.main.transform.parent.forward * 10;
                }
                if (Input.GetKey(KeyCode.S))
                {
                    transform.position += -Camera.main.transform.parent.forward * 10;
                }



                if (Input.GetKey(KeyCode.D))
                {

                    transform.position += Camera.main.transform.parent.right * 10;
                }
                if (Input.GetKey(KeyCode.A))
                {


                    transform.position += -Camera.main.transform.parent.right * 10;
                }

            }
        }
    }
}
