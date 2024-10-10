using Photon.Pun;
using UnityEngine.SceneManagement;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    // Offline mode flag
    public bool isOfflineMode = true;

    public void ConnectToTheServer()
    {
        // Offline mode ayarý
        if (isOfflineMode)
        {
            PhotonNetwork.OfflineMode = true;
            PhotonNetwork.NickName = "OfflinePlayer";
            OnConnectedToMaster();
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
            SceneManager.LoadSceneAsync("01_MainScene");  // Offline modda lobiye katýlmadan sahneyi yükle
        }
    }

    public override void OnJoinedLobby()
    {
        SceneManager.LoadSceneAsync("01_MainScene");
    }
}
