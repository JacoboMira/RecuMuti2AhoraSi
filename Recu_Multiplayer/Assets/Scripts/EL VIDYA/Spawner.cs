using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerJoseluis>())
        {
            other.GetComponent<PlayerJoseluis>().SetSpawner(this);
        }
    }
}
