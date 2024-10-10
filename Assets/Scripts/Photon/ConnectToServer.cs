using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    // Offline mode flag
    public bool isOfflineMode = true;

    public void ConnectToTheServer()
    {
        // Offline mode ayar�
        if (isOfflineMode)
        {
            PhotonNetwork.OfflineMode = true;
            PhotonNetwork.NickName = "OfflinePlayer";  // Offline modda kullan�c� ad�
            OnConnectedToMaster();  // Offline modda direk master'a ba�l� gibi davran
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.SendRate = 360;
            PhotonNetwork.SerializationRate = 360;
            PhotonNetwork.NickName = FirebaseManager.Instance.DisplayName;
        }
    }

    public override void OnConnectedToMaster()
    {
        if (!isOfflineMode)
        {
            PhotonNetwork.JoinLobby();
        }
        else
        {
            SceneManager.LoadSceneAsync("01_MainScene");  // Offline modda lobiye kat�lmadan sahneyi y�kle
        }
    }

    public override void OnJoinedLobby()
    {
        SceneManager.LoadSceneAsync("01_MainScene");
    }
}
