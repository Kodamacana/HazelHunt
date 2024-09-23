using System.Collections;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Unity.Cinemachine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    //[SerializeField] private GameObject crosshairSpriteObject; // Fare sprite'ý için oluþturulan nesne
    [Header("Panels & Managers")]
    [SerializeField] EndGamePanel endGamePanel;
    [SerializeField] MatchFoundPanelController matchFoundPanel;
    [SerializeField] ScoreManager scoreManager;
    [SerializeField] public CinemachineTargetGroup targetGroup;

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

    public string masterNickname;
    public string guestNickname;
    [HideInInspector] public bool isMatching = false;
    GameObject chosenTree;
    GameObject player;
    GameObject cloneObject;
    PhotonView view;

    [Header("Audio Clips")]
    public AudioClip sound_Bomb;
    public AudioClip sound_Shotgun;
    public AudioClip sound_Button;
    public AudioClip sound_Lose;
    public AudioClip sound_Win;
    public AudioClip sound_GettingShot;
    public AudioClip sound_NutCollect;

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
        view.RPC("MatchFound", RpcTarget.AllBuffered);
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
        healthText.text = "100";
        endGamePanel.gameObject.SetActive(false);

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
        return new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
    }

    //private void ShownCrossHair()
    //{
    //    Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //    mousePosition.z = 0f;
    //    crosshairSpriteObject.transform.position = mousePosition;
    //}

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

    public void SpawnPosition(GameObject player)
    {
        if (!view.IsMine)
            return;

        if (PhotonNetwork.IsMasterClient) player.transform.position = GetRandomPosition(minX2Y2.x, maxX2Y2.x, minX2Y2.y, maxX2Y2.y);
        else player.transform.position = GetRandomPosition(minX1Y1.x, maxX1Y1.x, minX1Y1.y, maxX1Y1.y);
    }
}
