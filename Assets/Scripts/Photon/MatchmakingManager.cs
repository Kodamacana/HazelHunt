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
    private bool isRematch = false;
    private bool readyRequestSent = false;
    private bool readyRequestReceived = false;
    private bool rematchRequestSent = false;
    private bool rematchRequestReceived = false;
    private float matchTimeout = 30f;

    FirebaseManager firebaseManager;
    FirestoreManager firestoreManager;
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
        firestoreManager = FirestoreManager.Instance;
        playerName = firebaseManager.UserName;
    }

    // Onclick -> playButton
    public void BeginMatchmaking()
    {
        StartMatchmaking();
    }
    #endregion


#region FindRandomMatch
    private void StartMatchmaking()
    {
        if (string.IsNullOrEmpty(playerName))
        {
            playerName = firebaseManager.UserName;
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
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && isMatching)
        {
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (!player.NickName.Equals(playerName))
                {
                    isMatching = true;
                    isRematch = false;
                    opponentNickname = player.NickName;
                    MainMenuController.instance.FindMatch(opponentNickname);
                    break;
                }
            }
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
                MaxPlayers = 2,
                PublishUserId = true
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
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (!player.NickName.Equals(playerName))
                {
                    opponentNickname = player.NickName;
                    MainMenuController.instance.FindMatch(opponentNickname);
                    break;
                }
            }
            //view.RPC("StartMatch", RpcTarget.All);
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
                view.RPC("StartMatch", RpcTarget.All);
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
            view.RPC("StartMatch", RpcTarget.All);
        }
    }
    public IEnumerator StartingMatch()
    {        
        yield return new WaitForSecondsRealtime(5f);
        PhotonNetwork.LoadLevel("GameScene");
    }

#endregion

    [PunRPC]
    private void StartMatch()
    {
        MainMenuController.instance.StartMatch();
        isMatching = false;
        readyRequestSent = false;
        readyRequestReceived = false;
        StartCoroutine(StartingMatch());
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
            IsVisible = true,
            MaxPlayers = 2,
            PublishUserId = true
        };
        PhotonNetwork.JoinOrCreateRoom(roomName,roomOptions,TypedLobby.Default); 
    }
   
    #endregion


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
