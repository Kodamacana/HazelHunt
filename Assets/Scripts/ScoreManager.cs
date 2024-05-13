using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class ScoreManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI player1ScoreText;
    [SerializeField] TextMeshProUGUI player2ScoreText;

    private int playerNumber;

    private int player1Score = 0;
    private int player2Score = 0;

    private void UpdateScoreUI()
    {
        player1ScoreText.text = player1Score.ToString();
        player2ScoreText.text = player2Score.ToString();
    }

    PhotonView view;

    private void Awake()
    {
        view = GetComponent<PhotonView>();        
    }
    private void Start()
    {
        UpdateScoreUI();
    }

    public void IncreasePlayerScore()
    {
        int playerNumber = GetPlayerNumber();
        if (playerNumber == 1)
        {
            player1Score++;
        }
        else if (playerNumber == 2)
        {
            player2Score++;
        }

        UpdateScoreUI();
        view.RPC("SyncScores", RpcTarget.Others, player1Score, player2Score);
    }
    private int GetPlayerNumber()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
        {
            return 1;
        }
        else if (PhotonNetwork.LocalPlayer.ActorNumber == 2)
        {
            return 2;
        }
        else
        {
            Debug.LogError("Invalid player number!");
            return -1;
        }
    }

    [PunRPC]
    private void SyncScores(int newPlayer1Score, int newPlayer2Score)
    {
        player1Score = newPlayer1Score;
        player2Score = newPlayer2Score;
        UpdateScoreUI();
    }
}
