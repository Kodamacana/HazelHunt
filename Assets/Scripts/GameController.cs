using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEditor;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    [SerializeField] private GameObject crosshairSpriteObject; // Fare sprite'ý için oluþturulan nesne


    public GameObject playerPrefab;

    public float minX1;
    public float maxX1;
    public float minY1;
    public float maxY1;

    public float minX2;
    public float maxX2;
    public float minY2;
    public float maxY2;

    PhotonView view;

    [SerializeField] GameObject baseTreePrefab;
    [SerializeField] GameObject nutsPoolPrefab;
    public GameObject chosenTree;
    public GameObject player;
    GameObject cloneObject;
    [SerializeField] ScoreManager scoreManager;
    [SerializeField] TextMeshProUGUI healthText;

    private void Awake()
    {
       Instance = this;
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
        view = GetComponent<PhotonView>();
        Cursor.visible = false;

        if (cloneObject == null)
        {
            cloneObject = PhotonNetwork.Instantiate(nutsPoolPrefab.name, Vector3.zero, Quaternion.identity);
        }

        if (PhotonNetwork.IsConnectedAndReady)
        {
            int playerNumber = GetPlayerNumber();
            if (playerNumber == 1)
            {
                player = PhotonNetwork.Instantiate(playerPrefab.name, GetRandomPosition(minX2, maxX2, minY2, maxY2), Quaternion.identity);

                chosenTree = PhotonNetwork.Instantiate(baseTreePrefab.name, new Vector3(-7.79f, 1.75f, 0), Quaternion.identity);
                StartCoroutine(GoBaseCollect(chosenTree, true));
            }
            else if (playerNumber == 2)
            {
                player = PhotonNetwork.Instantiate(playerPrefab.name, GetRandomPosition(minX1, maxX1, minY1, maxY1), Quaternion.identity);

                chosenTree = PhotonNetwork.Instantiate(baseTreePrefab.name, new Vector3(7.759f, 1.75f, 0), Quaternion.identity);
                StartCoroutine(GoBaseCollect( chosenTree, false));
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

    IEnumerator GoBaseCollect( GameObject obj, bool isFlipX)
    {
        while (obj.transform.GetChild(0) == null)
        {
            yield return null;
        }
        StartCoroutine(obj.GetComponentInChildren<BaseCollect>().WaitForObjectCreation(player, isFlipX));
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

    private void Update()
    {
        if (player != null)
        {
            if (player.GetComponent<PlayerBase>() != null)
            {
                int healthValue = player.GetComponent<PlayerBase>().currentHealth;
                if (healthValue <= 0) {
                    Die();
                }
                healthText.text = "+ "+ healthValue.ToString();
            }
        }
       
        ShownCrossHair();        
    }

    private void Die()
    {
        PhotonNetwork.Destroy(player);
        PhotonNetwork.Destroy(chosenTree);
        InitializeGame();
    }
}
