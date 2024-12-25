using System.Collections;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Unity.Cinemachine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    //[SerializeField] private GameObject crosshairSpriteObject; // Fare sprite'� i�in olu�turulan nesne
    [Header("Panels & Managers")]
    [SerializeField] EndGamePanel endGamePanel;
    [SerializeField] GameObject onGamePanel;
    [SerializeField] MatchFoundPanelController matchFoundPanel;
    [SerializeField] ScoreManager scoreManager;
    [SerializeField] private RichtapEffectSource source;

    [Header("Camera & Camera Groups")]
    [SerializeField] public CinemachineTargetGroup targetGroup;
    [SerializeField] private CinemachineCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin cameraNoise;
    private float shakeDuration = 0.1f;  
    private float shakeAmplitude = 1.2f; 
    private float shakeFrequency = 2.0f; 
    private float shakeTimer = 0f;

    [Header("Player Spawn Coordinates")]
    [SerializeField] Vector2 minX1Y1;
    [SerializeField] Vector2 maxX1Y1;
    [SerializeField] Vector2 minX2Y2;
    [SerializeField] Vector2 maxX2Y2;

    [Header("Joysticks")]
    public Joystick movementJoystick;
    public Joystick attackJoystick;
    public Joystick bombJoystick;

    [Header("Prefabs")]
    [SerializeField] GameObject baseTreePrefab;
    [SerializeField] GameObject nutsPoolPrefab;
    [SerializeField] GameObject playerPrefab;

    [Header("Text Mesh")]
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] TextMeshProUGUI killCountText;
    int killCount = 0;
    [SerializeField] GameObject killCounterImg;

    public string masterNickname;
    public string guestNickname;
    [HideInInspector] public bool isMatching = false;
    GameObject chosenTree;
    GameObject player;
    GameObject cloneObject;
    PhotonView view;

    [Header("Animators")]
    [SerializeField] public Animator bloodOverlayAnimator;
    [SerializeField] private GameObject killFeedBack;

    [Header("Audio Clips")]
    public AudioClip sound_Bomb;
    public AudioClip sound_Shotgun;
    public AudioClip sound_Button;
    public AudioClip sound_GettingShot;
    public AudioClip sound_NutCollect;

    public AudioClip sound_ImpactSplat1;
    public AudioClip sound_ImpactSplat2;
    public AudioClip sound_Lose;
    public AudioClip sound_Win;

    private void Awake()
    {
        Instance = this;
        view = GetComponent<PhotonView>();
    }

    private void Start()
    {
        InitializeGame();

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            MatchReady();
    }

    private void MatchReady()
    {
        view.RPC("MatchFound", RpcTarget.AllBufferedViaServer);
    }

    [PunRPC]
    public void MatchFound()
    {
        matchFoundPanel.gameObject.SetActive(true);
    }

    public void UpdateScore()
    {
        scoreManager.IncreasePlayerScore();
    }

    private void InitializeGame()
    {
        cameraNoise = virtualCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
        healthText.text = "100";
        endGamePanel.gameObject.SetActive(false);
        onGamePanel.SetActive(true);

        if (cloneObject == null && view.IsMine)
        {
            cloneObject = PhotonNetwork.Instantiate(nutsPoolPrefab.name, Vector3.zero, Quaternion.identity);
        }

        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                player = PhotonNetwork.Instantiate(playerPrefab.name, GetRandomPosition(minX2Y2.x, maxX2Y2.x, minX2Y2.y, maxX2Y2.y), Quaternion.identity);
               
                chosenTree = PhotonNetwork.Instantiate(baseTreePrefab.name, new Vector3(-7.79f, 1.75f, 0), Quaternion.identity);
                StartCoroutine(GoBaseCollect(chosenTree));
            }
            else
            {
                player = PhotonNetwork.Instantiate(playerPrefab.name, GetRandomPosition(minX1Y1.x, maxX1Y1.x, minX1Y1.y, maxX1Y1.y), Quaternion.identity);
                
                chosenTree = PhotonNetwork.Instantiate(baseTreePrefab.name, new Vector3(7.759f, 1.75f, 0), Quaternion.identity);
                StartCoroutine(GoBaseCollect( chosenTree));
            }
        }
        else
        {
            Debug.LogError("PhotonNetwork is not connected or ready!");
        }
    }
   

    IEnumerator GoBaseCollect( GameObject obj)
    {
        while (obj.transform.GetChild(0) == null)
        {
            yield return null;
        }
        StartCoroutine(obj.GetComponentInChildren<BaseCollect>().WaitForObjectCreation(player));
    }

    Vector2 GetRandomPosition(float minX, float maxX, float minY, float maxY)
    {
        return new Vector2(UnityEngine.Random.Range(minX, maxX), UnityEngine.Random.Range(minY, maxY));
    }

    public void DamagePlayer()
    {
        if (player.GetComponent<PlayerBase>() != null)
        {
            int healthValue = player.GetComponent<PlayerBase>().currentHealth;
            if (healthValue <= 0)
            {
                healthText.text = "0";
            }
            healthText.text = healthValue.ToString();                
        }
    }
    public IEnumerator IncreaseKillScore()
    {
        killFeedBack.SetActive(true);
        yield return new WaitForSecondsRealtime(3);
        killCount++;
        killCountText.text = killCount.ToString();
        killCounterImg.SetActive(true);
        killFeedBack.SetActive(false);
    }

    public void SpawnPosition(GameObject player)
    {
        if (!view.IsMine)
            return;

        if (PhotonNetwork.IsMasterClient) player.transform.position = GetRandomPosition(minX2Y2.x, maxX2Y2.x, minX2Y2.y, maxX2Y2.y);
        else player.transform.position = GetRandomPosition(minX1Y1.x, maxX1Y1.x, minX1Y1.y, maxX1Y1.y);
    }

    public void ShakeCamera(int sourceIndex = 0)
    {
        source.SelectEffect(sourceIndex, -1);
        source.Play();

        cameraNoise.AmplitudeGain = shakeAmplitude;
        cameraNoise.FrequencyGain = shakeFrequency;
        shakeTimer = shakeDuration;
    }

    void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0)
            {
                cameraNoise.AmplitudeGain = 0f;
            }
        }
    }

   
}
