using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections;
using Firebase.Firestore;
using System.Collections.Generic;
using System;
using Firebase.Extensions;

public class MatchmakingManager : MonoBehaviourPunCallbacks
{

#region Fields
    public static MatchmakingManager Instance;

    private WarningMessageManager warningMessageManager;
    private bool isMatching = false;
    private bool isFriend = false;
    private bool isRematch = false;
    private bool readyRequestSent = false;
    private bool readyRequestReceived = false;
    private bool rematchRequestSent = false;
    private bool rematchRequestReceived = false;
    private float matchTimeout = 30f;

    FirebaseManager firebaseManager;
    PhotonView view;

    bool isLeaveRoom = false;
    string playerName = "";
    string opponentNickname = "";
    #endregion


#region Initialize
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

        firebaseManager = FirebaseManager.Instance;
    }

    // Onclick -> playButton
    public void BeginMatchmaking()
    {
        if (PhotonNetwork.OfflineMode)
        {
            PhotonNetwork.JoinOrCreateRoom("OfflineModeTest", new RoomOptions(), TypedLobby.Default);
        }
        else if (PhotonNetwork.CurrentRoom != null)
        {
            PhotonNetwork.LeaveRoom();
            StartMatchmaking();
        }
        else StartMatchmaking();
    }
    #endregion


#region FindRandomMatch
    private void StartMatchmaking()
    {
        playerName = firebaseManager.DisplayName;
        if (string.IsNullOrEmpty(playerName))
        {
            playerName = firebaseManager.DisplayName;
            SetFeedback("Player name cannot be empty");
            return;
        }

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

        if (PhotonNetwork.CurrentRoom == null)
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
    #endregion


#region PhotonFunc
    public override void OnJoinedLobby()
    {
        isLeaveRoom = true;
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.OfflineMode)
        {
            PhotonNetwork.LoadLevel("GameScene");
        }
        else if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && isMatching)
        {
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (!player.NickName.Equals(playerName))
                {
                    isMatching = true;
                    isRematch = false;
                    opponentNickname = player.NickName;

                    MainMenuController.instance.FoundMatch(opponentNickname);

                    if (isFriend)
                    {
                        MainMenuController.instance.StartMatch();
                        SendReadyMatch();
                    }
                    break;
                }
            }
        }       
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        if (isMatching)
        {
            RoomOptions roomOptions = new RoomOptions()
            {
                MaxPlayers = 2
            };
            PhotonNetwork.JoinRandomOrCreateRoom(null, 2, MatchmakingMode.RandomMatching, TypedLobby.Default, null, null, roomOptions);
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
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (!player.NickName.Equals(playerName))
                {
                    opponentNickname = player.NickName;
                    MainMenuController.instance.FoundMatch(opponentNickname);

                    if (isFriend)
                    {
                        MainMenuController.instance.StartMatch();
                        SendReadyMatch();
                    }
                    break;
                }
            }
        }
    }
    #endregion


#region ReadyToStart
    public void SendReadyMatch()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && isMatching && !opponentNickname.Equals(""))
        {
            if (!readyRequestSent && !readyRequestReceived)
            {
                if (view != null)
                {
                    view.RPC("ReadyMatchRequest", RpcTarget.Others);
                    readyRequestSent = true;
                }
                else
                {
                    Debug.LogError("PhotonView is not assigned.");
                }
            }
            else if (readyRequestReceived)
            {
                closeTime = PhotonNetwork.Time + delay;
                view.RPC("StartMatch", RpcTarget.AllBufferedViaServer, closeTime);
            }
        }
    }

    [PunRPC]
    public void ReadyMatchRequest()
    {
        if (!readyRequestSent)
        {
            MainMenuController.instance.ClickedOpponent();
            readyRequestReceived = true;
        }
        else
        {
            closeTime = PhotonNetwork.Time + delay;
            view.RPC("StartMatch", RpcTarget.AllBufferedViaServer, closeTime);
        }
    }


    public float delay = 3f;
    private double closeTime;


    public IEnumerator StartingMatch(float delay)
    {        
        yield return new WaitForSecondsRealtime(delay);
        PhotonNetwork.LoadLevel("GameScene");
    }

    #endregion

    [PunRPC]
    private void StartMatch(double closeAt)
    {
        closeTime = closeAt;
        double timeRemaining = closeTime - PhotonNetwork.Time;
        isMatching = false;
        readyRequestSent = false;
        readyRequestReceived = false;

        if (timeRemaining > 0)
        {
            StartCoroutine(StartingMatch((float)timeRemaining));
        }
        MainMenuController.instance.StartMatch();
    }


#region Rematch
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
    #endregion


#region LeaveMatch
    [PunRPC]
    private void CloseRoom()
    {
        PhotonNetwork.LeaveRoom();
        SetFeedback("Leaving Room...");
    }
    public void ReturnToMainMenu()
    {
        SetFeedback("Returning to main menu...");
        isMatching = false;
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("01_MainScene");
    }

    #endregion


#region NextMatch
    public void NextMatch()
    {
        view.RPC("CloseRoom", RpcTarget.All);
        SetFeedback("Moving to next match...");
        isRematch = false; // Yeni maç olduðunda rematch deðil

        isMatching = true;
        StartCoroutine(MatchmakingCoroutine());
    }
    #endregion


    #region FriendMatch
    public void AcceptFriendMatchRequest(string roomName)
    {
        isMatching=true;
        RoomOptions roomOptions = new RoomOptions()
        {
            MaxPlayers = 2,
        };
        PhotonNetwork.JoinOrCreateRoom(roomName,roomOptions,TypedLobby.Default); 
    }
   
    #endregion


    private void SetFeedback(string message)
    {

#if UNITY_STANDALONE || UNITY_EDITOR
        if (warningMessageManager != null)
        {
            warningMessageManager.SetMessage(message);
        }
        else
        {
            Debug.LogError("WarningMessageManager is not set.");
        }
#endif
    }
}
