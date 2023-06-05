using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject spawner;
    // Start is called before the first frame update
    void Awake()
    {
        int randomInt = Random.Range(0, 3);
        switch (randomInt)
        {
            case 0:
                PhotonNetwork.Instantiate("Pez", spawner.transform.position, spawner.transform.rotation);
                break;
            case 1:
                PhotonNetwork.Instantiate("Pingo", spawner.transform.position, spawner.transform.rotation);
                break;
            case 2:
                PhotonNetwork.Instantiate("Rana", spawner.transform.position, spawner.transform.rotation);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
