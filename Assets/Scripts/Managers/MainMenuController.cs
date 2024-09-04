using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public static MainMenuController instance;
    [SerializeField] private Button playButton;
    public TMP_InputField playerName;

    MatchmakingManager matchmakingManager;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        matchmakingManager = MatchmakingManager.Instance;

        playButton.onClick.AddListener(delegate { matchmakingManager.BeginMatchmaking(); });
    }


}
