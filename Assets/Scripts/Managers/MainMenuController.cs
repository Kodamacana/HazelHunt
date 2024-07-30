using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button playButton;

    MatchmakingManager matchmakingManager;

    private void Start()
    {
        matchmakingManager = MatchmakingManager.Instance;

        playButton.onClick.AddListener(delegate { matchmakingManager.StartMatchmaking(); });
    }
}
