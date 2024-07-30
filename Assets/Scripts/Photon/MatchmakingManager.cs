using Photon.Pun;
using TMPro;
using Photon.Realtime;
using UnityEngine;
using System.Collections;

public class MatchmakingManager : MonoBehaviourPunCallbacks
{
    public static MatchmakingManager Instance;
    public TMP_InputField playerName;
    public PhotonView photonView;
    private WarningMessageManager warningMessageManager;
    private bool isMatching = false;
    private bool isRematch = false;
    private bool rematchRequestSent = false;
    private bool rematchRequestReceived = false;
    private float matchTimeout = 15f;

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
        {
            Debug.LogError("WarningMessageManager instance is not set.");
        }

        if (photonView == null)
        {
            photonView = GetComponent<PhotonView>();
        }

        InitializeUI();
    }

    private void InitializeUI()
    {
        playerName.text = "";
    }

    // Onclick -> playButton
    public void StartMatchmaking()
    {
        if (string.IsNullOrEmpty(playerName.text))
        {
            SetFeedback("Player name cannot be empty");
            return;
        }

        PhotonNetwork.NickName = playerName.text;
        SetFeedback("Starting matchmaking...");
        isMatching = true;
        StartCoroutine(MatchmakingCoroutine());
    }

    private IEnumerator MatchmakingCoroutine()
    {
        float startTime = Time.time;
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
            isMatching = false;
            isRematch = false; // Yeni maç baþladýðýnda rematch deðil
            PhotonNetwork.LoadLevel("GameScene");
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
            SetFeedback("Player found! Starting game...");
            isMatching = false;
            PhotonNetwork.LoadLevel("GameScene");
        }
    }

    public void SendRematchRequest()
    {
        if (!rematchRequestSent && !rematchRequestReceived)
        {
            if (photonView != null)
            {
                photonView.RPC("ReceiveRematchRequest", RpcTarget.Others);
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
        photonView.RPC("Rematch", RpcTarget.All);        
    }

    [PunRPC]
    private void Rematch()
    {
        SetFeedback("Rematch accepted. Starting new match...");
        isRematch = true;
        rematchRequestSent = false;
        rematchRequestReceived = false;
        PhotonNetwork.LoadLevel("GameScene");
    }

    public void NextMatch()
    {
        SetFeedback("Moving to next match...");
        isRematch = false; // Yeni maç olduðunda rematch deðil
        PhotonNetwork.LeaveRoom();
        StartMatchmaking();
    }

    public void ReturnToMainMenu()
    {
        SetFeedback("Returning to main menu...");
        isMatching = false;
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("MainMenu");
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
