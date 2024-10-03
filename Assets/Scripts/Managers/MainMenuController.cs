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

    [SerializeField] GameObject readyPanel;
    [SerializeField] GameObject opponentSquirrelObject;
    [SerializeField] GameObject mySquirrelObject;
    private Animator readyPanelAnimator;
    private Animator opponentSquirrelAnimator;
    private Animator mySquirrelAnimator;
 
    [Header("Material")]
    [SerializeField] Material blackMaterial;
    [SerializeField] Material normalMaterial;

    MatchmakingManager matchmakingManager;
    FirebaseManager firebaseManager;

    bool isMyReady = false;
    bool isOpponentReady = false;

    private void Awake()
    {
        instance = this;
        firebaseManager = FirebaseManager.Instance;
        matchmakingManager = MatchmakingManager.Instance;

        readyPanelAnimator = readyPanel.GetComponent<Animator>();
        mySquirrelAnimator = mySquirrelObject.GetComponent<Animator>();
        opponentSquirrelAnimator = opponentSquirrelObject.GetComponent<Animator>();

        myReadyButton.onClick.RemoveAllListeners();
        huntButton.onClick.RemoveAllListeners();
    }

    private void Start()
    {
        mySquirrelObject.SetActive(false);
        opponentSquirrelObject.SetActive(false);

        imageMyReadyButton.material = normalMaterial;
        imageOpponentReadyButton.material = normalMaterial;

        nuts_txt.text = firebaseManager.Nut.ToString();
        score_txt.text = firebaseManager.Score.ToString();
        myUsername.text = firebaseManager.UserName;
        mySquirrelObject.SetActive(true);

        huntButton.onClick.AddListener(delegate { matchmakingManager.BeginMatchmaking(); });
    }

    public void FindMatch(string opponentUsername)
    {
        matchmakingPanel.SetActive(true);
        this.opponentUsername.text = opponentUsername;
        opponentSquirrelObject.SetActive(true);
        readyPanelAnimator.SetTrigger("FoundOpponent");
        //Düþman karakterin geldiði bir animasyon ekle
        myReadyButton.onClick.AddListener(delegate { ClickReady();});
    }

    private void ClickReady()
    {
        isMyReady = true;
        mySquirrelAnimator.SetTrigger("SquirrelReady");
        matchmakingManager.SendReadyMatch();
        imageMyReadyButton.material = blackMaterial;
    }

    public void ClickedOpponent()
    {
        isOpponentReady = true;
        imageOpponentReadyButton.material = blackMaterial;
        opponentSquirrelAnimator.SetTrigger("SquirrelReady");
    }

    public void StartMatch()
    {
        if (!isMyReady)
        {
            imageMyReadyButton.material = blackMaterial;
            mySquirrelAnimator.SetTrigger("SquirrelReady");
        }
        else if (!isOpponentReady)
        {
            imageOpponentReadyButton.material = blackMaterial;
            opponentSquirrelAnimator.SetTrigger("SquirrelReady");
        }
    }
}
