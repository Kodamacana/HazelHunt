using Firebase.Firestore;
using Photon.Pun;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InvitePopUp : MonoBehaviour
{
    [SerializeField] Button acceptButton;
    [SerializeField] Button declineButton;
    [SerializeField] TextMeshProUGUI usernameText;

    public string fromUserId;
    public string fromUsername;

    private void Awake()
    {
        acceptButton.onClick.AddListener(delegate { FirestoreManager.Instance.AcceptMatchRequest(); });
        declineButton.onClick.AddListener(delegate { FirestoreManager.Instance.RejectMatchRequest(); });
    }

    public void ShowMatchRequestPopup(string fromUsername, string fromUserId)
    {
        this.fromUserId = fromUserId;
        this.fromUsername = fromUsername;

        usernameText.text = fromUsername;

        if (!SceneManager.GetActiveScene().name.Contains("Game"))
         gameObject.SetActive(true);
    }

}
