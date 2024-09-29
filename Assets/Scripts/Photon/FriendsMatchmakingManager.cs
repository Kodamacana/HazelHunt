using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class FriendsMatchmakingManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public static FriendsMatchmakingManager Instance { get; private set; }
    public const byte InviteEventCode = 1;  // Davet olay kodu

    PhotonView view;
    private void Awake()
    {
        Instance = this;

        if (view == null)
            view = GetComponent<PhotonView>();
    }
    public void SendInvite(string invitedPlayerName)
    {
        if (!PhotonNetwork.InRoom)
        {
            // Oda ad� daveti g�nderenin kullan�c� ad� olsun
            string roomName = PhotonNetwork.NickName;

            // Oda olu�turma ayarlar�
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 2;  // Oyun i�in maksimum 2 oyuncu
            PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);  // Oday� olu�tur

            // Oda olu�turulduktan sonra daveti g�nder (odadayken RaiseEvent kullan�labilir)
            object[] content = new object[] { PhotonNetwork.NickName, invitedPlayerName, roomName };

            // Daveti RaiseEvent ile kar�� tarafa g�nder
            PhotonNetwork.RaiseEvent(InviteEventCode, content, RaiseEventOptions.Default, SendOptions.SendReliable);
        }
        else
        {
            Debug.LogWarning("Zaten bir odadas�n�z!");
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        if (eventCode == InviteEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            string invitingPlayerName = (string)data[0];  // Daveti g�nderen oyuncunun ad�
            string invitedPlayerName = (string)data[1];   // Davet edilen oyuncunun ad�
            string roomName = (string)data[2];            // Davet g�nderenin olu�turdu�u oda ad�

            // E�er bu davet edilen oyuncu bizsek, daveti kabul edip odaya kat�lal�m
            if (invitedPlayerName == PhotonNetwork.NickName)
            {
                ShowInvitePopup(invitingPlayerName, roomName);  // Popup g�ster ve odaya kat�lma se�ene�i sun
            }
        }
    }

    private void ShowInvitePopup(string invitingPlayerName, string roomName)
    {
        OnAcceptInvite(roomName);
        //invitePopup.SetActive(true);  // UI'da popup'� aktif et
        //inviteMessageText.text = invitingPlayerName + " seni oyuna davet ediyor!";  // Popup mesaj�n� ayarla

        //acceptButton.onClick.AddListener(() => OnAcceptInvite(roomName));  // Kabul et butonuna t�klan�nca OnAcceptInvite �al���r
    }

    public void OnAcceptInvite(string roomName)
    {
        if (!PhotonNetwork.InRoom)
        {
            PhotonNetwork.JoinRoom(roomName);  // Daveti g�nderenin odas�na kat�l
        }
        else
        {
            Debug.LogWarning("Zaten bir odadas�n�z!");
        }
    }

    void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

   
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            view.RPC("StartMatch", RpcTarget.All);
        }
    }

    [PunRPC]
    private void StartMatch()
    {
        PhotonNetwork.LoadLevel("GameScene");
    }
}
