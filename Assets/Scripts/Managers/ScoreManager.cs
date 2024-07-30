using Photon.Pun;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI player1ScoreText;
    [SerializeField] TextMeshProUGUI player2ScoreText;
    PhotonView view;

    private int player1Score = 0;
    private int player2Score = 0;

    private void Awake()
    {
        view = GetComponent<PhotonView>();
    }

    private void Start()
    {
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        player1ScoreText.text = player1Score.ToString();
        player2ScoreText.text = player2Score.ToString();
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

    public string GetEndGameScore()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber.Equals(1))
        {
            return player1Score + "-" + player2Score;
        }
        else
        {
            return player2Score + "-" + player1Score;
        }        
    }

    public int GetWinnerScore()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber.Equals(1))
        {
            if (player1Score > player2Score)
            {
                return 1;
            }
            else if (player1Score < player2Score)
            {
                return 2;
            }
            return 0;
        }
        else
        {
            if (player1Score < player2Score)
            {
                return 1;
            }
            else if (player1Score > player2Score)
            {
                return 2;
            }
            return 0;
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
