using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public static MainMenuController instance;
    [SerializeField] private Button playButton;
    [SerializeField] private TextMeshProUGUI nuts_txt;
    [SerializeField] private TextMeshProUGUI score_txt;

    MatchmakingManager matchmakingManager;
    FirebaseManager firebaseManager;

    private void Awake()
    {
        instance = this;
        firebaseManager = FirebaseManager.Instance;
        matchmakingManager = MatchmakingManager.Instance;
    }

    private void Start()
    {
        nuts_txt.text = firebaseManager.Nut.ToString();
        score_txt.text = firebaseManager.Score.ToString();

        playButton.onClick.AddListener(delegate { matchmakingManager.BeginMatchmaking(); });
    }
}
