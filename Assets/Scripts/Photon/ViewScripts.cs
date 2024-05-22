using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using TMPro;


public class ViewScripts : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI serverName;

    public void CreateLobby()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinOrCreateRoom(serverName.text, new Photon.Realtime.RoomOptions() { MaxPlayers = 2 }, null);
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = "1.0.0";
        }
    }
}

