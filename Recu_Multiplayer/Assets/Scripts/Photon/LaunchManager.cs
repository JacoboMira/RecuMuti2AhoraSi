using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Random = UnityEngine.Random;

public class LaunchManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject enterGamePanel;
    [SerializeField] private GameObject connectionStatusPanel;
    [SerializeField] private GameObject lobbyPanel;

    private void Start()
    {
        enterGamePanel.SetActive(true);
        connectionStatusPanel.SetActive(false);
        lobbyPanel.SetActive(false);
    }

// Update is called once per frame
    void Update()
    {

    }

// Método para comprueba que si estamos conectado a los servidores de Photon y en caso de no estar
// conectados utilizamos los UsingSettings servidores de Photon y en caso de no estar
// conectados utilizamos los UsingSettings
    public void ConnectToPhotonServer()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            enterGamePanel.SetActive(false);
            connectionStatusPanel.SetActive(true);
        }
    }

// Método que trata de conectar al usuario a una Room aleatoria, en caso de no existir la lógica se ejecutara en
// el override OnJoinRandomFailed
    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

// Called when the client is connected to the Master Server and ready for matchmaking and other tasks.
    public override void OnConnectedToMaster()
    {
        Debug.Log("Conectando a los servidores de Photon con el usuario " + PhotonNetwork.NickName);
        lobbyPanel.SetActive(true);
        connectionStatusPanel.SetActive(false);
    }

// Called to signal that the raw connection got established but before the client can call operation on the server.
    public override void OnConnected()
    {
        Debug.Log("Conectando a Internet");
    }

    public override void OnJoinRandomFailed(short returnCode,
        string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        Debug.Log(message);
        CreateAndJoinRoom();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.NickName + " se ha conectado a " +
                  PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " se ha conectado a " +
                  PhotonNetwork.CurrentRoom.Name +

                  " -- Numero players:" +
                  PhotonNetwork.CurrentRoom.PlayerCount);
    }

// Método para crear un nuevo Room con el nombre aleatorio y los parámetros definidos de base.
    private void CreateAndJoinRoom()
    {
        string randomRoomName = "Room " + Random.Range(0, 10000);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 20;
        PhotonNetwork.CreateRoom(randomRoomName, roomOptions);
    }
}