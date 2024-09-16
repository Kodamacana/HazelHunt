using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections;

public class MatchmakingManager : MonoBehaviourPunCallbacks
{
    public static MatchmakingManager Instance;
        
    private WarningMessageManager warningMessageManager;
    private bool isMatching = false;
    private bool isRematch = false;
    private bool rematchRequestSent = false;
    private bool rematchRequestReceived = false;
    private float matchTimeout = 15f;

    MainMenuController mainMenuController;
    AuthManager authManager;
    FirebaseManager firebaseManager;
    PhotonView view;

    bool isLeaveRoom = false;
    string playerName = "";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        warningMessageManager = WarningMessageManager.instance;

        if (warningMessageManager == null)
            Debug.LogError("WarningMessageManager instance is not set.");

        if (view == null)
            view = GetComponent<PhotonView>();

    }

    // Onclick -> playButton
    public void BeginMatchmaking() {
        mainMenuController = MainMenuController.instance;
        authManager = AuthManager.Instance;
        firebaseManager = FirebaseManager.Instance;
        playerName = firebaseManager.DisplayName;

        StartMatchmaking();
    }

    private void StartMatchmaking()
    {
        if (string.IsNullOrEmpty(playerName))
        {
            playerName = authManager.UserId;
            SetFeedback("Player name cannot be empty");
            return;
        }

        PhotonNetwork.NickName = playerName;
        SetFeedback("Starting matchmaking...");

        isMatching = true;
        isLeaveRoom = true;
        StartCoroutine(MatchmakingCoroutine());
    }

    IEnumerator MatchmakingCoroutine()
    {
        float startTime = Time.time;
        float leaveDelayTime = Time.time - 3;

        while (!isLeaveRoom && Time.time - leaveDelayTime > matchTimeout)
        {
            yield return null;
        }

        PhotonNetwork.JoinRandomRoom();

        while (isMatching && Time.time - startTime < matchTimeout)
        {
            yield return null;
        }

        if (isMatching)
        {
            SetFeedback("Matchmaking failed. No players found.");
            PhotonNetwork.LeaveRoom();
            isMatching = false;
        }
    }    

    public override void OnJoinedRoom()
    {
        if (isMatching)
        {
            SetFeedback("Match found! Starting game...");
            isMatching = true;
            isRematch = false; // Yeni maç baþladýðýnda rematch deðil
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        if (isMatching)
        {
            // If failed to join a random room, create a new room
            RoomOptions roomOptions = new RoomOptions()
            {                
                IsVisible = true,
                MaxPlayers = 2
            };
            PhotonNetwork.CreateRoom(null, roomOptions, TypedLobby.Default);
        }
    }

    public override void OnCreatedRoom()
    {
        if (isMatching)
        {
            SetFeedback("Created room, waiting for another player...");
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && isMatching)
        {
            view.RPC("StartMatch", RpcTarget.All);
        }
    }

    public void SendRematchRequest()
    {
        if (!rematchRequestSent && !rematchRequestReceived)
        {
            if (view != null)
            {
                view.RPC("ReceiveRematchRequest", RpcTarget.Others);
                SetFeedback("Rematch request sent. Waiting for response...");
                rematchRequestSent = true;
            }
            else
            {
                Debug.LogError("PhotonView is not assigned.");
            }
        }
        else if (rematchRequestReceived)
        {
            AcceptRematchRequest();
        }
    }

    [PunRPC]
    public void ReceiveRematchRequest()
    {
        if (!rematchRequestSent)
        {
            SetFeedback("Rematch request received. Press rematch button to accept.");
            rematchRequestReceived = true;
        }
        else
        {
            AcceptRematchRequest();
        }
    }

    private void AcceptRematchRequest()
    {
        view.RPC("ReMatch", RpcTarget.All);        
    }

    [PunRPC]
    private void ReMatch()
    {
        SetFeedback("Rematch accepted. Starting new match...");
        isRematch = true;
        rematchRequestSent = false;
        rematchRequestReceived = false;
        PhotonNetwork.LoadLevel("GameScene");
    }

    [PunRPC]
    private void StartMatch()
    {
        SetFeedback("Match accepted. Starting new match...");
        isMatching = false;
        PhotonNetwork.LoadLevel("GameScene");
    }

    [PunRPC]
    private void CloseRoom()
    {
        PhotonNetwork.LeaveRoom();
        SetFeedback("Leaving Room...");
    }

    public void NextMatch()
    {
        view.RPC("CloseRoom", RpcTarget.All);
        SetFeedback("Moving to next match...");
        isRematch = false; // Yeni maç olduðunda rematch deðil

        isMatching = true;
        StartCoroutine(MatchmakingCoroutine());
    }

    public override void OnJoinedLobby()
    {
        isLeaveRoom = true;
    }

    public void ReturnToMainMenu()
    {
        SetFeedback("Returning to main menu...");
        isMatching = false;
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("01_MainScene");
    }

    private void SetFeedback(string message)
    {
        if (warningMessageManager != null)
        {
            warningMessageManager.SetMessage(message);
        }
        else
        {
            Debug.LogError("WarningMessageManager is not set.");
        }
    }
}
