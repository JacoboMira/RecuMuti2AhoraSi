using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
public class PlayerNameInputManager : MonoBehaviour
{
    
// Creamos un método para setear el nombre del jugador a partir de los datos que obtenemos del InputField de la UI
   
    public void SetPlayerName(string playername)
    {
        if (string.IsNullOrEmpty(playername))
        {
            Debug.Log("El nombre del jugador está vacío");
            return;
        }
        PhotonNetwork.NickName = playername;
    }
}