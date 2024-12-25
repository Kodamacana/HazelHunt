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

    [SerializeField]
    Sprite[] WinLoseDrawText_EN;

    string[] WinLoseDrawText_TR =
    {
        "Berabere!", "Kazandýn!", "Ezik hA hA"
    };

    [SerializeField] TextMeshProUGUI txtMasterUsername;
    [SerializeField] TextMeshProUGUI txtGuestUsername;
    //[SerializeField] TextMeshProUGUI txtEarnedCoins;
    [SerializeField] TextMeshProUGUI txtEarnedNuts;
    [SerializeField] TextMeshProUGUI txtScore;
    [SerializeField] Image winLoseImg;
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

        int winLoseDrawEnum = scoreManager.GetWinnerScore();

        winLoseImg.sprite = WinLoseDrawText_EN[winLoseDrawEnum];

        if (winLoseDrawEnum == 1)
        {
            SoundManagerSO.PlaySoundFXClip(GameController.Instance.sound_Win, transform.position, 1f);
        }
        else
        {
            SoundManagerSO.PlaySoundFXClip(GameController.Instance.sound_Lose, transform.position, 1f);
        }

        txtMasterUsername.text = gameController.masterNickname;
        txtGuestUsername.text = gameController.guestNickname;

        rematchButton.onClick.AddListener(delegate { matchmakingManager.SendRematchRequest(); });
        nextMatchButton.onClick.AddListener(delegate { matchmakingManager.NextMatch(); });
        returnToMenuButton.onClick.AddListener(delegate { matchmakingManager.ReturnToMainMenu(); });
    }
}
