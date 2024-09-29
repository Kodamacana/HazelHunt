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
            // Oda adý daveti gönderenin kullanýcý adý olsun
            string roomName = PhotonNetwork.NickName;

            // Oda oluþturma ayarlarý
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 2;  // Oyun için maksimum 2 oyuncu
            PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);  // Odayý oluþtur

            // Oda oluþturulduktan sonra daveti gönder (odadayken RaiseEvent kullanýlabilir)
            object[] content = new object[] { PhotonNetwork.NickName, invitedPlayerName, roomName };

            // Daveti RaiseEvent ile karþý tarafa gönder
            PhotonNetwork.RaiseEvent(InviteEventCode, content, RaiseEventOptions.Default, SendOptions.SendReliable);
        }
        else
        {
            Debug.LogWarning("Zaten bir odadasýnýz!");
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        if (eventCode == InviteEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            string invitingPlayerName = (string)data[0];  // Daveti gönderen oyuncunun adý
            string invitedPlayerName = (string)data[1];   // Davet edilen oyuncunun adý
            string roomName = (string)data[2];            // Davet gönderenin oluþturduðu oda adý

            // Eðer bu davet edilen oyuncu bizsek, daveti kabul edip odaya katýlalým
            if (invitedPlayerName == PhotonNetwork.NickName)
            {
                ShowInvitePopup(invitingPlayerName, roomName);  // Popup göster ve odaya katýlma seçeneði sun
            }
        }
    }

    private void ShowInvitePopup(string invitingPlayerName, string roomName)
    {
        OnAcceptInvite(roomName);
        //invitePopup.SetActive(true);  // UI'da popup'ý aktif et
        //inviteMessageText.text = invitingPlayerName + " seni oyuna davet ediyor!";  // Popup mesajýný ayarla

        //acceptButton.onClick.AddListener(() => OnAcceptInvite(roomName));  // Kabul et butonuna týklanýnca OnAcceptInvite çalýþýr
    }

    public void OnAcceptInvite(string roomName)
    {
        if (!PhotonNetwork.InRoom)
        {
            PhotonNetwork.JoinRoom(roomName);  // Daveti gönderenin odasýna katýl
        }
        else
        {
            Debug.LogWarning("Zaten bir odadasýnýz!");
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
