using Photon.Pun;
using TMPro;
using Photon.Realtime;
using UnityEngine;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    public TMP_InputField createLobby;
    public TMP_InputField playerName;

    //Onclick -> createButton
    public void CreateRoom()
    {
        PhotonNetwork.NickName = playerName.text;

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsVisible = false;
        roomOptions.MaxPlayers = 2;
        PhotonNetwork.JoinOrCreateRoom(createLobby.text, roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("GameScene");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);

        Debug.LogError(message +" "+ returnCode);
    }
}
