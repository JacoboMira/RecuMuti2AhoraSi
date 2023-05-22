using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LaunchManager : MonoBehaviourPunCallbacks
{
  // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
  // Update is called once per frame
    void Update()
    {
        
    }
  // Called when the client is connected to the Master Server and ready for matchmaking and other tasks.
    
    public override void OnConnectedToMaster()
    {
        Debug.Log("Conectando a los servidores de Photon");
    }

  // Called to signal that the raw connection got established but before the client can call operation on the server.
   
    public override void OnConnected()
    {
        Debug.Log("Conectando a Internet");
    }
}