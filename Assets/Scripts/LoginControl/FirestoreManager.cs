﻿using UnityEngine;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Collections.Generic;
using System;
using Firebase.Auth;
using System.Threading.Tasks;
using System.Linq;
using System.Runtime.CompilerServices;

[FirestoreData]
public struct UserData
{
    [FirestoreProperty]
    public string UserName { get; set; }

    [FirestoreProperty]
    public List<object> LoginList { get; set; }

    [FirestoreProperty]
    public string Mail { get; set; }

    [FirestoreProperty]
    public Timestamp Created { get; set; }

    [FirestoreProperty]
    public string PhotoUrl { get; set; }

    [FirestoreProperty]
    public bool OnlineStatus { get; set; }
}

[FirestoreData]
public struct Progress
{
    [FirestoreProperty]
    public int Nut { get; set; }

    [FirestoreProperty]
    public int Score { get; set; }

    [FirestoreProperty]
    public List<object> Friends_user_list { get; set; }

    [FirestoreProperty]
    public List<object> Friendship_invites_list { get; set; }
}

public class FirestoreManager : MonoBehaviour
{
    public static FirestoreManager Instance;

    private ListenerRegistration _listenerUserData;

    [SerializeField] private string UserId;
    [SerializeField] private ConnectToServer ConnectToServer;
    private FirebaseFirestore firestore;
    public readonly List<string> onlineUsersList = new List<string>();

    Progress progress;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        firestore = FirebaseFirestore.DefaultInstance;
    }

    private void OnDestroy()
    {
        _listenerUserData.Stop();
    }

    private async void UpdateOnlineUsersList()
    {
        await GetOnlineUsers();
        InvokeRepeating(nameof(UpdateOnlineUsersList), 0f, 30f);

        Debug.Log("Online Users Count: " + onlineUsersList.Count);
    }

    /// <summary>
    /// GetOnlineUsers her client tarafindan gonderilen istek olmasi nedeniyle firestore'u zorlayabilir. Server tarafina cekilebilmesi denenecektir.
    /// </summary>
    /// <returns></returns>
    private async Task GetOnlineUsers()
    {
        onlineUsersList.Clear();

        try
        {
            QuerySnapshot snapshot = await firestore.Collection("Users")
                                             .WhereEqualTo("online_status", true)
                                             .GetSnapshotAsync();

            foreach (var document in snapshot.Documents)
            {
                string userId = document.Id;
                onlineUsersList.Add(userId);

                Debug.Log("Online User ID: " + userId);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error fetching online users: " + e.Message);
        }
    }

    public async Task GetFirestoreDatas(string userId = "", bool isLogin = false)
    {
        if (!string.IsNullOrEmpty(userId))
        {
            this.UserId = userId;
        }

        firestore = FirebaseFirestore.DefaultInstance;

        // Firestore'dan snapshot'ı asenkron olarak alıyoruz
        DocumentSnapshot snapshot = await firestore.Collection("Users").Document(this.UserId).GetSnapshotAsync();

        if (!snapshot.Exists)
        {
            CreateNewUser(this.UserId);
        }
        else
        {
            Dictionary<string, object> personalData = snapshot.GetValue<Dictionary<string, object>>("personal_data");

            if (isLogin)
            {
                GetUserData(personalData);
                LoadScene();
            }
            GetProgressData(snapshot.ToDictionary());

            Debug.Log("User document already exists");
        }
    }


    private void CreateNewUser(string UserId)
    {
        var authManager = AuthManager.Instance;
        Dictionary<string, object> userData = new Dictionary<string, object>
        {
            { "personal_data", new Dictionary<string, object>
                {
                    { "created", authManager.CreatedDate},
                    { "mail", authManager.Email},
                    { "name", authManager.DisplayName},
                    { "photo", authManager.PhotoUrl},
                    { "online_status", true},
                    { "login_list", new List<object> { Timestamp.FromDateTime(System.DateTime.UtcNow) } }
                }
            },
            { "nut", 0 },
            { "score", 0 },
            { "friendship_invites_list", new List<string>() },
            { "friends_user_list", new List<string>() }

        };

        DocumentReference docRef = firestore.Collection("Users").Document(UserId);
        docRef.SetAsync(userData).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                PlayerPrefs.SetInt("isFirstGame", 0);
                LoadScene();
                Debug.Log("User document successfully created!");
            }
            else
            {
                Debug.LogError("Failed to create user document: " + task.Exception);
            }
        });
    }
    private bool ConvertToBool(object value)
    {
        if (value is bool)
            return (bool)value;

        if (value is string strValue)
        {
            return strValue.ToLower() == "true";
        }

        if (value is int intValue)
        {
            return intValue == 1;
        }

        return false;
    }

    private void GetUserData(Dictionary<string, object> personalData)
    {
        AuthManager authManager = AuthManager.Instance;
        FirebaseUser user = authManager.Auth.CurrentUser;

        UserData userData = new()
        {
            UserName = personalData["name"].ToString(),
            Mail = personalData["mail"].ToString(),
            LoginList = personalData["login_list"] as List<object>,
            PhotoUrl = personalData["photo"].ToString(),
            OnlineStatus = personalData.ContainsKey("online_status") ? ConvertToBool(personalData["online_status"]) : false,
            Created = new Timestamp()
        };

        FirebaseManager.Instance.SaveLocalFromFirestoreInUserDatas(UserId, userData.UserName, userData.Mail, userData.LoginList, userData.PhotoUrl, userData.OnlineStatus);

        userData.LoginList.Add(Timestamp.FromDateTime(DateTime.UtcNow));
        personalData["created"] = FirebaseManager.Instance.UserRegisterDate;
        personalData["login_list"] = userData.LoginList;
        personalData["online_status"] = true;

        Dictionary<string, object> updatedUserData = new Dictionary<string, object>
                    {
                        { "personal_data", personalData }
                    };

        firestore.Collection("Users").Document(UserId).UpdateAsync(updatedUserData).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("User document successfully updated!");
            }
            else
            {
                Debug.LogError("Failed to update user document: " + task.Exception);
            }
        });
    }

    private void GetProgressData(Dictionary<string, object> progressData)
    {
        progress = new()
        {
            Nut = (int)(long)progressData["nut"],
            Score = (int)(long)progressData["score"],
            Friends_user_list = progressData["friends_user_list"] as List<object>,
            Friendship_invites_list = progressData["friendship_invites_list"] as List<object>
        };

        FirebaseManager.Instance.SaveLocalFromFirestoreInProgressDatas(progress.Nut, progress.Score, progress.Friendship_invites_list, progress.Friends_user_list);
    }
    
    public void SetNut(int value)
    {
        progress.Nut += value;
        Dictionary<string, object> updatedCoin = new()
        {
            { "nut", progress.Nut}
        };

        firestore.Collection("Users").Document(UserId).UpdateAsync(updatedCoin);
    }
    
    public void SetScore(int value)
    {
        progress.Score = value;
        Dictionary<string, object> updatedScore = new()
        {
            { "score", progress.Score }
        };
        firestore.Collection("Users").Document(UserId).UpdateAsync(updatedScore);
    }

    public async void SendFriendRequestByUsername(string username)
    {
        CollectionReference usersCollection = firestore.Collection("Users");

        Query query = usersCollection.WhereEqualTo("personal_data.name",  username);
        try
        {
            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

            foreach (DocumentSnapshot document in querySnapshot.Documents)
            {
                var userDoc = document;
                var userId = userDoc.Id;

                AuthManager authManager = AuthManager.Instance;

                if (userId != authManager.UserId)
                {
                    await userDoc.Reference.UpdateAsync("friendship_invites_list", FieldValue.ArrayUnion(authManager.UserId));

                    Debug.Log("Arkadaş isteği başarıyla gönderildi.");
                }
                else
                {
                    Debug.Log("Kendinize arkadaş isteği gönderemezsiniz.");
                }
            }
        }
        catch (Exception e)
        {
            // Hata durumunda hata mesajı yazdırıyoruz
            Debug.LogError($"Hata oluştu: {e.Message}");
        }
    }


    public async void AcceptFriendRequest(string userId)
    {
        try
        {
            var currentUserDocRef = firestore.Collection("Users").Document(this.UserId);

            var requestingUserDocRef = firestore.Collection("Users").Document(userId);

            await currentUserDocRef.UpdateAsync("friends_user_list", FieldValue.ArrayUnion(userId));

            await requestingUserDocRef.UpdateAsync("friends_user_list", FieldValue.ArrayUnion(this.UserId));

            await currentUserDocRef.UpdateAsync("friendship_invites_list", FieldValue.ArrayRemove(userId));

            Debug.Log("Arkadaşlık isteği kabul edildi.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Arkadaşlık isteğini kabul ederken hata oluştu: {e.Message}");
        }
    }

    public async void RejectFriendRequest(string userId)
    {
        try
        {
            var currentUserDocRef = firestore.Collection("Users").Document(this.UserId);

            await currentUserDocRef.UpdateAsync("friendship_invites_list", FieldValue.ArrayRemove(userId));

            Debug.Log("Arkadaşlık isteği reddedildi.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Arkadaşlık isteğini reddederken hata oluştu: {e.Message}");
        }
    }


    public async Task<List<FriendData>> GetFriendsData()
    {
        await GetFirestoreDatas(UserId, false);

        List<FriendData> friendData = new List<FriendData>();
        if (progress.Friends_user_list == null)
            return null;

        foreach (string userId in progress.Friends_user_list)
        {
            DocumentReference userDocRef = firestore.Collection("Users").Document(userId);
        
            // Firestore'dan snapshot'ı asenkron olarak alıyoruz
            DocumentSnapshot documentSnapshot = await userDocRef.GetSnapshotAsync();

            if (documentSnapshot.Exists)
            {
                var personal_data = documentSnapshot.GetValue<Dictionary<string, object>>("personal_data");

                string name = personal_data["name"].ToString();
                string photoUrl = personal_data["photo"].ToString();
                bool onlineStatus = false;
                if (personal_data.ContainsKey("online_status"))
                {
                    onlineStatus = ConvertToBool(personal_data["online_status"]);
                }
                int score = documentSnapshot.GetValue<int>("score");
                int nut = documentSnapshot.GetValue<int>("nut");

                friendData.Add(new(name, photoUrl, score, nut, onlineStatus, userId));
            }
            else
            {
                Debug.Log("User " + userId + " does not exist in Firestore.");
            }
        }

        return friendData;
    }

    public async Task<List<FriendData>> GetInvitesDataAsync()
    {
        await GetFirestoreDatas(UserId, false);

        List<FriendData> friendData = new List<FriendData>();

        if (progress.Friendship_invites_list == null)
            return null;

        foreach (string userId in progress.Friendship_invites_list)
        {
            DocumentReference userDocRef = firestore.Collection("Users").Document(userId);

            DocumentSnapshot documentSnapshot = await userDocRef.GetSnapshotAsync();

            if (documentSnapshot.Exists)
            {
                var personal_data = documentSnapshot.GetValue<Dictionary<string, object>>("personal_data");

                string name = personal_data["name"].ToString();
                string photoUrl = personal_data["photo"].ToString();

                //bool onlineStatus = false;
                //if (personal_data.ContainsKey("online_status"))
                //{
                //    onlineStatus = ConvertToBool(personal_data["online_status"]);
                //}

                int score = documentSnapshot.GetValue<int>("score");
                int nut = documentSnapshot.GetValue<int>("nut");

                friendData.Add(new(name, photoUrl, score, nut, true, userId));
            }
            else
            {
                Debug.Log("User " + userId + " does not exist in Firestore.");
            }
        }

        return friendData;
    }        


    private void OnApplicationQuit()
    {
        SetOnlineStatus(false);
    }

    private void SetOnlineStatus(bool isOnline)
    {
        DocumentReference docRef = firestore.Collection("Users").Document(this.UserId);

        Dictionary<string, object> personalData = new Dictionary<string, object>
    {
        { "online_status", isOnline } 
    };

        Dictionary<string, object> updatedUserData = new Dictionary<string, object>
    {
        { "personal_data", personalData } 
    };

        // Veriyi Firestore'a kaydetme işlemi
        docRef.SetAsync(updatedUserData, SetOptions.MergeAll).ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Online durumu Firestore'a başarıyla kaydedildi.");
            }
            else
            {
                Debug.LogError("Online durumu Firestore'a kaydedilemedi: " + task.Exception);
            }
        });
    }


    public void LoadScene()
    {
        ConnectToServer.ConnectToTheServer();
        //SceneManager.LoadSceneAsync(1);
    }
}