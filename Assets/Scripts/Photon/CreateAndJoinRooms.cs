using Photon.Pun;
using TMPro;
using Photon.Realtime;
using UnityEngine;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    public TMP_InputField createLobby;
    public TMP_InputField playerName;

    private void Start()
    {
        createLobby.text = "Test Room";
        playerName.text = "1-) Profesyonel 31ci";
    }

    //Onclick -> createButton
    public void CreateRoom()
    {
        PhotonNetwork.NickName = playerName.text;

        RoomOptions roomOptions = new()
        {
            IsVisible = false,
            MaxPlayers = 2
        };
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
