using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public static MainMenuController instance;
    [SerializeField] private Button huntButton;
    [SerializeField] private TextMeshProUGUI nuts_txt;
    [SerializeField] private TextMeshProUGUI score_txt;

    [Header("Matchmaking Panel")]
    [SerializeField] GameObject matchmakingPanel;
    [SerializeField] Button myReadyButton;
    [SerializeField] Image imageMyReadyButton;
    [SerializeField] Image imageOpponentReadyButton;
    [SerializeField] TextMeshProUGUI myUsername;
    [SerializeField] TextMeshProUGUI opponentUsername;

    [Header("Material")]
    [SerializeField] Material blackMaterial;
    [SerializeField] Material normalMaterial;

    MatchmakingManager matchmakingManager;
    FirebaseManager firebaseManager;

    private void Awake()
    {
        instance = this;
        firebaseManager = FirebaseManager.Instance;
        matchmakingManager = MatchmakingManager.Instance;

        myReadyButton.onClick.RemoveAllListeners();
        huntButton.onClick.RemoveAllListeners();
    }

    private void Start()
    {
        imageMyReadyButton.material = normalMaterial;
        imageOpponentReadyButton.material = normalMaterial;

        nuts_txt.text = firebaseManager.Nut.ToString();
        score_txt.text = firebaseManager.Score.ToString();
        myUsername.text = firebaseManager.UserName;

        huntButton.onClick.AddListener(delegate { matchmakingManager.BeginMatchmaking(); });
    }

    public void FindMatch(string opponentUsername)
    {
        this.opponentUsername.text = opponentUsername;
        //Ready Buttonlar�n�n ��kt��� bir animasyon ekle
        //D��man karakterin geldi�i bir animasyon ekle
        myReadyButton.onClick.AddListener(delegate { ClickReady();});
    }

    private void ClickReady()
    {
        //bizim sincap ate� etme animasyonu
        matchmakingManager.SendReadyMatch();
        imageMyReadyButton.material = blackMaterial;
    }

    public void ClickedOpponent()
    {
        imageOpponentReadyButton.material = blackMaterial;
        //d��man sincap ate� etme animasyonu
    }

    public IEnumerator StartingMatch()
    {
        yield return new WaitForSecondsRealtime(3f);
    }
}
