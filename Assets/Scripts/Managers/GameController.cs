using System.Collections;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    [SerializeField] private GameObject crosshairSpriteObject; // Fare sprite'ý için oluþturulan nesne

    public GameObject playerPrefab;

    [SerializeField] Vector2 minX1Y1;
    [SerializeField] Vector2 maxX1Y1;
    [SerializeField] Vector2 minX2Y2;
    [SerializeField] Vector2 maxX2Y2;

    [SerializeField] GameObject baseTreePrefab;
    [SerializeField] GameObject nutsPoolPrefab;
    [SerializeField] ScoreManager scoreManager;
    [SerializeField] TextMeshProUGUI healthText;
    [HideInInspector] public GameObject chosenTree;
    [HideInInspector] public GameObject player;
    GameObject cloneObject;
    PhotonView view;

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
        Cursor.visible = false;

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

    private void ShownCrossHair()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;
        crosshairSpriteObject.transform.position = mousePosition;
    }

    public void DamagePlayer()
    {
        if (player.GetComponent<PlayerBase>() != null)
        {
            int healthValue = player.GetComponent<PlayerBase>().currentHealth;
            if (healthValue <= 0)
            {
                Die();
                healthText.text = "+ 100";
            }
            healthText.text = "+ " + healthValue.ToString();
        }
    }

    private void Die()
    {
        PhotonNetwork.Destroy(player);
        PhotonNetwork.Destroy(chosenTree);
        InitializeGame();
    }

    private void Update()
    {
        ShownCrossHair();
    }
}
