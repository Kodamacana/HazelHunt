using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    public TMP_InputField createLobby;
    public TMP_InputField playerName;

    public void CreateRoom()
    {
        PhotonNetwork.NickName = playerName.text;
        try
        {
            PhotonNetwork.CreateRoom(createLobby.text);
        }
        catch (System.Exception)
        {
            JoinRoom();
            throw;
        }
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(createLobby.text);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("GameScene");
    }
}
