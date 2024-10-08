using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    public void ConnectToTheServer()
    {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.SendRate = 360;
        PhotonNetwork.SerializationRate = 360;
        PhotonNetwork.NickName = FirebaseManager.Instance.DisplayName;
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        SceneManager.LoadSceneAsync("01_MainScene");
    }
}
