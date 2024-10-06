using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InvitesManager : MonoBehaviour
{
    public static InvitesManager Instance;

    FirestoreManager firestoreManager;
    List<FriendData> inviteList = null;

    [SerializeField] FriendObject friendObjectPrefab;
    [SerializeField] List<FriendObject> friendObjectList;
    [SerializeField] GameObject invitePanel;
    [SerializeField] Button inviteButton;
    [SerializeField] Button acceptInvite;
    [SerializeField] Button declineInvite;
    [SerializeField] public GameObject inviteValueObject;
    [SerializeField] public TextMeshProUGUI inviteValue_txt;

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
        firestoreManager = FirestoreManager.Instance;
    }

    private void Start()
    {
        inviteButton.onClick.AddListener(delegate { CreateInviteObject(); });
        acceptInvite.onClick.AddListener(delegate { RequestEvaluation(true); });
        declineInvite.onClick.AddListener(delegate { RequestEvaluation(false); });

        for (int i = 0; i < friendObjectList.Count; i++)
        {
            friendObjectList[i].GetComponent<BoxCollider2D>().enabled = true;
            friendObjectList[i].selectedUI.gameObject.SetActive(false);
        }
    }

    private void ResetPanel()
    {
        choosenUserId = "";
        acceptInvite.enabled = false;
        declineInvite.enabled = false;
        acceptInvite.GetComponent<Image>().material = blackSpriteMaterial;
        declineInvite.GetComponent<Image>().material = blackSpriteMaterial;

        for (int i = 0; i < friendObjectList.Count; i++)
        {
            friendObjectList[i].GetComponent<BoxCollider2D>().enabled = true;
            friendObjectList[i].selectedUI.gameObject.SetActive(false);
        }
    }

    private void RequestEvaluation(bool isAccept)
    {
        if (choosenUserId == "")
            return;

        var friendData = inviteList.Find(x => x.UserId.Equals(choosenUserId));
        inviteList.Remove(friendData);
        var friendObject = friendObjectList.Find(x => x.UID.Equals(choosenUserId));
        Destroy(friendObject.gameObject);
        friendObjectList.Remove(friendObject);

        if (isAccept)
        {
            firestoreManager.AcceptFriendRequest(choosenUserId);
        }
        else
        { 
            firestoreManager.RejectFriendRequest(choosenUserId);
        }
    }

    private async void CreateInviteObject()
    {
        try
        {
            ResetPanel();
            inviteList = await firestoreManager.GetFriendsData(true);
            choosenUserId = "";
            int onlineUserCount = 0;
            for (int i = 0; i < inviteList.Count; i++)
            {
                var alreadyExists = friendObjectList.Where(x => x.UID.Equals(inviteList[i].UserId)).ToList();

                string username = inviteList[i].Username;
                string score = inviteList[i].Score.ToString();
                string nut = inviteList[i].Nut.ToString();
                bool isOnline = true;
                string UID = inviteList[i].UserId;

                if (isOnline) onlineUserCount++;

                if (alreadyExists.Count > 0)
                {
                    alreadyExists[0].Init(username, score, nut, isOnline, UID);
                    continue;
                }

                FriendObject friendObject = Instantiate(friendObjectPrefab, invitePanel.transform);
                friendObject.Init(username, score, nut, isOnline, UID);
                friendObjectList.Add(friendObject);
            }
        }
        catch (System.Exception)
        {
            throw;
        }
        
    }

    public void SelectFriendObject(FriendObject friend, string UID)
    {
        choosenUserId = UID;

        acceptInvite.enabled = true;
        declineInvite.enabled = true;
        acceptInvite.GetComponent<Image>().material = normalMaterial;
        declineInvite.GetComponent<Image>().material = normalMaterial;

        for (int i = 0; i < friendObjectList.Count; i++)
        {
            if (friendObjectList[i].Equals(friend))
                continue;

            friendObjectList[i].GetComponent<BoxCollider2D>().enabled = true;
            friendObjectList[i].selectedUI.gameObject.SetActive(false);
        }
    }
}
