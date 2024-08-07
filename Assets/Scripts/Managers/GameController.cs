using System.Collections;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    //[SerializeField] private GameObject crosshairSpriteObject; // Fare sprite'� i�in olu�turulan nesne
    [Header("Panels & Managers")]
    [SerializeField] EndGamePanel endGamePanel;
    [SerializeField] MatchFoundPanelController matchFoundPanel;
    [SerializeField] ScoreManager scoreManager;

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

    [HideInInspector] public string masterNickname;
    [HideInInspector] public string guestNickname;
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
    }

    public void UpdateScore()
    {
        scoreManager.IncreasePlayerScore();
    }

    private void InitializeGame()
    {
        healthText.text = "100";
        endGamePanel.gameObject.SetActive(false);
        matchFoundPanel.gameObject.SetActive(true);

        if (cloneObject == null && view.IsMine)
        {
            cloneObject = PhotonNetwork.Instantiate(nutsPoolPrefab.name, Vector3.zero, Quaternion.identity);
        }

        if (PhotonNetwork.IsConnectedAndReady)
        {
            int playerNumber = GetPlayerNumber();
            if (playerNumber == 1)
            {
                player = PhotonNetwork.Instantiate(playerPrefab.name, GetRandomPosition(minX2Y2.x, maxX2Y2.x, minX2Y2.y, maxX2Y2.y), Quaternion.identity);
               
                chosenTree = PhotonNetwork.Instantiate(baseTreePrefab.name, new Vector3(-7.79f, 1.75f, 0), Quaternion.identity);
                StartCoroutine(GoBaseCollect(chosenTree));
            }
            else if (playerNumber == 2)
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

    private int GetPlayerNumber()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == 2)
        {
            return 1;
        }
        else if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
        {
            return 2;
        }
        else
        {
            Debug.LogError("Invalid player number!");
            return -1;
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

        int playerNumber = GetPlayerNumber();

        if (playerNumber == 1) player.transform.position = GetRandomPosition(minX2Y2.x, maxX2Y2.x, minX2Y2.y, maxX2Y2.y);
        else player.transform.position = GetRandomPosition(minX1Y1.x, maxX1Y1.x, minX1Y1.y, maxX1Y1.y);
    }
}
