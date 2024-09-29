using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class FriendsMatchmakingManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public static FriendsMatchmakingManager Instance { get; private set; }
    private const byte INVITE_EVENT = 1; // Davet g�nderme olay�
    private const byte ACCEPT_INVITE_EVENT = 2; // Daveti kabul etme olay�

    [HideInInspector] public string friendUsername;  // Davet g�ndermek istedi�iniz ki�inin kullan�c� ad�

    private void Awake()
    {
        Instance = this;
    }

    // Davet g�nderme fonksiyonu
    public void SendInvite()
    {
        // Kullan�c� ad� ile birlikte davet g�nderiyoruz
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

    // Davet veya kabul olaylar�n� i�leyen callback
    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        if (eventCode == INVITE_EVENT)
        {
            object[] data = (object[])photonEvent.CustomData;
            string inviter = (string)data[0];

            // Popup ekran�nda davet mesaj�
            Debug.Log(inviter + " seni oyuna davet ediyor!");

            // Bu k�sm� kullan�c�ya bir popup g�sterecek �ekilde geni�letebilirsiniz
            // �rn: Popup'ta "Daveti kabul et" butonu g�sterilir ve kabul edilirse AcceptInvite() �a�r�l�r.
        }
        else if (eventCode == ACCEPT_INVITE_EVENT)
        {
            object[] data = (object[])photonEvent.CustomData;
            string accepter = (string)data[0];

            Debug.Log(accepter + " davetini kabul etti!");

            // Davet kabul edildi�inde oyuncular ayn� odaya yerle�tirilir
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
        Debug.Log("Odaya kat�ld�n�z: " + PhotonNetwork.CurrentRoom.Name);
        // Oyun ba�lat�labilir
        PhotonNetwork.LoadLevel("GameScene"); // Oyun sahnesini y�kleyin
    }
}
