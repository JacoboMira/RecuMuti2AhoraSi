using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject player;
    // Start is called before the first frame update

    private void Start()
    {
        transform.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (player.GetComponent<Rigidbody>().velocity.y > -70)
        {
            transform.position = player.transform.position;
        }
        else
        {
            transform.LookAt(player.transform.position);
        }
    }
}
