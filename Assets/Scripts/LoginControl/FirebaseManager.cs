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
    [SerializeField] private string phone;
    [SerializeField] private string photoUrl;
    [SerializeField] private List<object> loginList;
    [SerializeField] private Timestamp userRegisterDate;
    [SerializeField] public int[] PowerupLevelIds;
    [SerializeField] private int nut;
    [SerializeField] private int highScore;
    [SerializeField] private int score;

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
    public string Phone
    {
        get => phone;
        private set => phone = value;
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
    public int Nut
    {
        get => nut;
        private set => nut = value >= 0 ? value : throw new ArgumentException("nut must be non-negative");
    }
    public int HighScore
    {
        get => highScore;
        private set => highScore = value >= 0 ? value : throw new ArgumentException("HighScore must be non-negative");
    }
    public int Score
    {
        get => score;
        private set => score = value >= 0 ? value : throw new ArgumentException("Score must be non-negative");
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

    public void SaveLocalFromFirestoreInUserDatas(string userName, string displayName, string mail, string phone, List<object> loginList, string photoUrl)
    {
        AuthManager authManager = AuthManager.Instance;
        FirebaseUser user = authManager.Auth.CurrentUser;

        UserName = userName;
        DisplayName = displayName;
        Mail = mail;
        Phone = phone;
        LoginList = loginList;
        PhotoUrl = photoUrl;

        long dateValue = long.Parse(user.Metadata.CreationTimestamp.ToString());
        var dateTime = DateTimeOffset.FromUnixTimeMilliseconds(dateValue).UtcDateTime;
        Timestamp timestamp = Timestamp.FromDateTime(dateTime);
        UserRegisterDate = timestamp;
    }

    public void SaveLocalFromFirestoreInProgressDatas(int nut, int highScore, int score)
    {
        Nut = nut;
        HighScore = highScore;
        Score = score;
    }
}