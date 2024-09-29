using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class FriendsMatchmakingManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public static FriendsMatchmakingManager Instance { get; private set; }
    private const byte INVITE_EVENT = 1; // Davet gönderme olayý
    private const byte ACCEPT_INVITE_EVENT = 2; // Daveti kabul etme olayý

    [HideInInspector] public string friendUsername;  // Davet göndermek istediðiniz kiþinin kullanýcý adý

    private void Awake()
    {
        Instance = this;
    }

    // Davet gönderme fonksiyonu
    public void SendInvite()
    {
        // Kullanýcý adý ile birlikte davet gönderiyoruz
        object[] content = new object[] { PhotonNetwork.NickName };
        RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(INVITE_EVENT, content, options, SendOptions.SendReliable);
    }

    // Daveti kabul etme fonksiyonu
    public void AcceptInvite()
    {
        object[] content = new object[] { PhotonNetwork.NickName };
        RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(ACCEPT_INVITE_EVENT, content, options, SendOptions.SendReliable);
    }

    // Davet veya kabul olaylarýný iþleyen callback
    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        if (eventCode == INVITE_EVENT)
        {
            object[] data = (object[])photonEvent.CustomData;
            string inviter = (string)data[0];

            // Popup ekranýnda davet mesajý
            Debug.Log(inviter + " seni oyuna davet ediyor!");

            // Bu kýsmý kullanýcýya bir popup gösterecek þekilde geniþletebilirsiniz
            // Örn: Popup'ta "Daveti kabul et" butonu gösterilir ve kabul edilirse AcceptInvite() çaðrýlýr.
        }
        else if (eventCode == ACCEPT_INVITE_EVENT)
        {
            object[] data = (object[])photonEvent.CustomData;
            string accepter = (string)data[0];

            Debug.Log(accepter + " davetini kabul etti!");

            // Davet kabul edildiðinde oyuncular ayný odaya yerleþtirilir
            RoomOptions roomOptions = new RoomOptions { MaxPlayers = 2 };
            PhotonNetwork.JoinOrCreateRoom("MatchRoom_" + PhotonNetwork.NickName + "_" + accepter, roomOptions, TypedLobby.Default);
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

    public override void OnJoinedRoom()
    {
        Debug.Log("Odaya katýldýnýz: " + PhotonNetwork.CurrentRoom.Name);
        // Oyun baþlatýlabilir
        PhotonNetwork.LoadLevel("GameScene"); // Oyun sahnesini yükleyin
    }
}
