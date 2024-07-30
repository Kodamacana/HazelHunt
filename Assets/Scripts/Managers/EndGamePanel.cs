using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndGamePanel : MonoBehaviour
{
    public static EndGamePanel instance;

    enum WinLoseDraw
    {
        Draw,
        Win,
        Lose
    }

    string[] WinLoseDrawText_EN = 
    {
        "Draw!", "You Win!", "Loser hA hA"
    };

    string[] WinLoseDrawText_TR =
    {
        "Berabere!", "Kazandýn!", "Ezik hA hA"
    };

    [SerializeField] TextMeshProUGUI txtMasterUsername;
    [SerializeField] TextMeshProUGUI txtGuestUsername;
    [SerializeField] TextMeshProUGUI txtEarnedCoins;
    [SerializeField] TextMeshProUGUI txtEarnedNuts;
    [SerializeField] TextMeshProUGUI txtScore;
    [SerializeField] TextMeshProUGUI txtWinLose;
    [SerializeField] ScoreManager scoreManager;

    [SerializeField] RawImage guestPlayerRawImg;
    [SerializeField] RawImage masterPlayerRawImg;

    [Header("Buttons")]
    [SerializeField] Button rematchButton;
    [SerializeField] Button nextMatchButton;
    [SerializeField] Button returnToMenuButton;

    GameController gameController;
    MatchmakingManager matchmakingManager;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        gameController = GameController.Instance;
        matchmakingManager = MatchmakingManager.Instance;

        txtScore.text = scoreManager.GetEndGameScore();

        WinLoseDraw winLoseDrawEnum = (WinLoseDraw)scoreManager.GetWinnerScore();

        txtWinLose.text = "";
        txtWinLose.text = WinLoseDrawText_EN[(int)winLoseDrawEnum];

        txtMasterUsername.text = gameController.masterNickname;
        txtGuestUsername.text = gameController.guestNickname;

        rematchButton.onClick.AddListener(delegate { matchmakingManager.SendRematchRequest(); });
        nextMatchButton.onClick.AddListener(delegate { matchmakingManager.NextMatch(); });
        returnToMenuButton.onClick.AddListener(delegate { matchmakingManager.ReturnToMainMenu(); });
    }
}
