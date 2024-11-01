using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendshipManager : MonoBehaviour
{
    public static FriendshipManager Instance;
    FirestoreManager firestoreManager;
    List<FriendData> friendsList = null;

    [SerializeField] GameObject friendListPanel;
    [SerializeField] FriendObject friendObjectPrefab;
    [SerializeField] List<FriendObject> friendObjectList;
    [SerializeField] TMP_InputField friendUsernameText;
    [SerializeField] TextMeshProUGUI clickedUsernameText;
    [SerializeField] TextMeshProUGUI onlineUsers;
    [SerializeField] Button friendListOpenButton;
    [SerializeField] Button inviteCloseButton;
    [SerializeField] Button sendFriendInvitationButton;
    [SerializeField] Button huntWithFriendButton;
    [SerializeField] Image huntWithFriendButtonIcon;
    [SerializeField] Animator rightAreaAnimator;

    [Header("Materials")]
    [SerializeField] private Material blackSpriteMaterial;
    [SerializeField] private TMP_FontAsset blackFontMaterial;

    [Space(10)]
    [SerializeField] private Material normalMaterial;
    [SerializeField] private TMP_FontAsset normalFontMaterial;

    string choosenUserId = "";
       
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        firestoreManager = FirestoreManager.Instance;
        sendFriendInvitationButton.onClick.AddListener(delegate { SendFriendInvitation(); });
        friendListOpenButton.onClick.AddListener(delegate { CreateFriendObjectAsync(); });
        inviteCloseButton.onClick.AddListener(delegate { CreateFriendObjectAsync(); });
    }

    private void ResetPanel()
    {
        rightAreaAnimator.SetBool("Open", false);
        rightAreaAnimator.SetBool("Close",true);
        choosenUserId = "";
        huntWithFriendButton.enabled = false;
        huntWithFriendButton.GetComponent<Image>().material = blackSpriteMaterial;
        huntWithFriendButtonIcon.material = blackSpriteMaterial;

        for (int i = 0; i < friendObjectList.Count; i++)
        {
            friendObjectList[i].GetComponent<BoxCollider2D>().enabled = true;
            friendObjectList[i].selectedUI.gameObject.SetActive(false);
        }
    }

    private void SendFriendInvitation()
    {
        firestoreManager.SendFriendRequestByUsername(friendUsernameText.text);
    }

    public async void CreateFriendObjectAsync()
    { 
        try
        {
            firestoreManager = FirestoreManager.Instance;
            ResetPanel();
            friendsList = await firestoreManager.GetFriendsData(false);

            if (friendsList != null && friendsList.Count > 0)
            {
                choosenUserId = "";
                int onlineUserCount = 0;
                for (int i = 0; i < friendsList.Count; i++)
                {
                    var alreadyExists = friendObjectList.Where(x => x.UID.Equals(friendsList[i].UserId)).ToList();

                    string username = friendsList[i].Username;
                    string score = friendsList[i].Score.ToString();
                    string nut = friendsList[i].Nut.ToString();
                    bool isOnline = friendsList[i].OnlineStatus;
                    string UID = friendsList[i].UserId;

                    if (isOnline) onlineUserCount++;

                    if (alreadyExists.Count > 0)
                    {
                        alreadyExists[0].Init(username, score, nut, isOnline, UID);
                        continue;
                    }

                    FriendObject friendObject = Instantiate(friendObjectPrefab, friendListPanel.transform);
                    friendObject.Init(username, score, nut, isOnline, UID);
                    friendObjectList.Add(friendObject);
                }
                onlineUsers.text = "(" + onlineUserCount + "/" + friendsList.Count.ToString() + ")";
            }
            onlineUsers.text = "";
        }
        catch (System.Exception)
        {
            throw;
        }        
    }   

    public void SelectFriendObject(FriendObject friend, string UID, string username)
    {
        rightAreaAnimator.SetBool("Close", false);
        rightAreaAnimator.SetBool("Open", true);
        choosenUserId = UID;
        clickedUsernameText.text = username;
        huntWithFriendButton.enabled = true;
        huntWithFriendButton.onClick.AddListener(delegate { SendMatchRequest(UID); });
        huntWithFriendButton.GetComponent<Image>().material = normalMaterial;
        huntWithFriendButtonIcon.material = normalMaterial;

        for (int i = 0; i < friendObjectList.Count; i++)
        {
            if (friendObjectList[i].Equals(friend))
                continue;

            friendObjectList[i].GetComponent<BoxCollider2D>().enabled = true;
            friendObjectList[i].selectedUI.gameObject.SetActive(false);
        }        
    }

    private void SendMatchRequest(string opponentUserId)
    {
        firestoreManager.SendMatchRequest(opponentUserId);
    }
}
