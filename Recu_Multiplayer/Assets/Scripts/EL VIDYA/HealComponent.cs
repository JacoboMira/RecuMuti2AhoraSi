using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealComponent : MonoBehaviour
{
    PlayerJoseluis player;
    private void Awake()
    {
        player = transform.parent.GetComponent<PlayerJoseluis>();
        transform.parent = null;
    }

    private void Update()
    {
        transform.position = player.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerJoseluis>() && other.GetComponent<PlayerJoseluis>() != this)
        {
            player.playersToHeal.Add(other.GetComponent<PlayerJoseluis>());
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerJoseluis>())
        {
            player.playersToHeal.Remove(other.GetComponent<PlayerJoseluis>());
        }
    }
}
