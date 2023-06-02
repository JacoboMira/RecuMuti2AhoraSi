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
        PhotonNetwork.Instantiate("Player", spawner.transform.position, spawner.transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
