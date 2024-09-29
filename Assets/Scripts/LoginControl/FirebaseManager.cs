using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance;

    [Header("Information of Database datas")]
    [SerializeField] private string userName;
    [SerializeField] private string displayName;
    [SerializeField] private string mail;
    [SerializeField] private string photoUrl;
    [SerializeField] private bool isOnline;
    [SerializeField] private List<object> loginList;
    [SerializeField] private Timestamp userRegisterDate;
    [SerializeField] private int nut;
    [SerializeField] private int score;
    [SerializeField] private List<object> friendship_invites_list;
    [SerializeField] private List<object> friends_user_list;

    public string UserName
    {
        get => userName;
        private set => userName = string.IsNullOrEmpty(value) ? throw new ArgumentException("UserName cannot be null or empty") : value;
    }
    public string DisplayName
    {
        get => displayName;
        private set => displayName = value;
    }
    public string Mail
    {
        get => mail;
        private set => mail = value;
    }
    public List<object> LoginList
    {
        get => loginList;
        private set => loginList = value ?? throw new ArgumentNullException(nameof(LoginList), "LoginList cannot be null");
    }
    public Timestamp UserRegisterDate
    {
        get => userRegisterDate;
        private set => userRegisterDate = value;
    }
    public string PhotoUrl
    {
        get => photoUrl;
        private set => photoUrl = value;
    }
    public bool OnlineStatus
    {
        get => isOnline;
        private set => isOnline = value;
    }
    public int Nut
    {
        get => nut;
        private set => nut = value >= 0 ? value : throw new ArgumentException("nut must be non-negative");
    }
    public int Score
    {
        get => score;
        private set => score = value >= 0 ? value : throw new ArgumentException("Score must be non-negative");
    }
    public List<object> Friendship_invites_list 
    { 
        get => friendship_invites_list;
        private set => friendship_invites_list = value;
    }
    public List<object> Friends_user_list 
    {
        get => friends_user_list; 
        private set => friends_user_list = value;
    }

    [Header("Managers")]
    [SerializeField] AuthManager authManager;
    [SerializeField] FirestoreManager firestoreManager;


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
        Firebase.FirebaseApp.CheckDependenciesAsync().ContinueWithOnMainThread(task => {
            if (task.Result == Firebase.DependencyStatus.Available)
            {
                authManager.gameObject.SetActive(true);
                firestoreManager.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + task.Result);
            }
        });
    }
   

    public void SaveLocalFromFirestoreInUserDatas(string userName, string displayName, string mail, List<object> loginList, string photoUrl, bool isOnline)
    {
        AuthManager authManager = AuthManager.Instance;
        FirebaseUser user = authManager.Auth.CurrentUser;

        UserName = userName;
        DisplayName = displayName;
        Mail = mail;
        LoginList = loginList;
        PhotoUrl = photoUrl;
        OnlineStatus = isOnline;

        long dateValue = long.Parse(user.Metadata.CreationTimestamp.ToString());
        var dateTime = DateTimeOffset.FromUnixTimeMilliseconds(dateValue).UtcDateTime;
        Timestamp timestamp = Timestamp.FromDateTime(dateTime);
        UserRegisterDate = timestamp;
    }

    public void SaveLocalFromFirestoreInProgressDatas(int nut, int score, List<object> friendship_invites_list, List<object> friends_user_list)
    {
        Nut = nut;
        Score = score;
        Friendship_invites_list = friendship_invites_list;
        Friends_user_list = friends_user_list;
    }
}