using UnityEngine;
using Firebase.Extensions;
using Firebase.Auth;
using Firebase.Firestore;
using System;

#if (UNITY_IOS || UNITY_TVOS)
using UnityEngine.SocialPlatforms.GameCenter;
#elif(UNITY_ANDROID)
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using System.Threading.Tasks;
#endif


public class AuthManager : MonoBehaviour
{
    public static AuthManager Instance;

    [Header("Authenticated User Info")]
    [SerializeField] private string _userId;
    [SerializeField] private string _displayName;
    [SerializeField] private Timestamp _createdDate;
    [SerializeField] private string _email;
    [SerializeField] private string _photoUrl;

    public string UserId
    {
        get => _userId;
        private set => _userId = string.IsNullOrEmpty(value) ? throw new ArgumentException("UserName cannot be null or empty") : value;
    }
    public string DisplayName
    {
        get => _displayName;
        private set => _displayName = value;
    }
    public string Email
    {
        get => _email;
        private set => _email = value;
    }
    public Timestamp CreatedDate
    {
        get => _createdDate;
        private set => _createdDate = value;
    }
    public string PhotoUrl
    {
        get => _photoUrl;
        private set => _photoUrl = value;
    }

    string authCode;
    [SerializeField] public FirebaseAuth Auth { get; set; }

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

    private void OnEnable()
    {
        InitializeAuth();
    }

    private void InitializeAuth()
    {
        Auth = FirebaseAuth.DefaultInstance;

#if UNITY_ANDROID
        GooglePlayLogin();
#elif UNITY_IOS || UNITY_TVOS
        AuthenticateToGameCenter();
#elif UNITY_STANDALONE
        SignInEmail();
#endif
    }

#if UNITY_ANDROID

    private void GooglePlayLogin()
    {
        PlayGamesPlatform.Activate();
        Social.localUser.Authenticate((success) =>
        {
            if (success)
            {
                Debug.Log("Google Play Games: Logged In");
                ConnectToFirebaseWithGPGS();
            }
            else
            {
                Debug.LogError("Google Play Games login failed");
                //Auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
                //{
                //    if (task.IsCanceled)
                //    {
                //        Debug.LogError("SignInAnonymouslyAsync was canceled.");
                //        return;
                //    }
                //    if (task.IsFaulted)
                //    {
                //        Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                //        return;
                //    }

                //    AuthResult result = task.Result;
                //    Debug.LogFormat("User signed in successfully: {0} ({1})",
                //        result.User.DisplayName, result.User.UserId);

                //    AnonymouslyLoginSuccess(result.User.UserId);
                //});
            }
        });
    }

    //private void AnonymouslyLoginSuccess(string anonymouslyUserId)
    //{
    //    if (string.IsNullOrEmpty(PlayerPrefs.GetString("AnonymouslyID")))
    //    {
    //        PlayerPrefs.SetString("AnonymouslyID", anonymouslyUserId);
    //    }

    //    anonymouslyUserId = PlayerPrefs.GetString("AnonymouslyID");
    //    FirestoreManager.Instance.GetFirestoreDatas(anonymouslyUserId, true);
    //}

    private void ConnectToFirebaseWithGPGS()
    {
        PlayGamesPlatform.Instance.RequestServerSideAccess(true, code =>
        {
            authCode = code;
            Credential credential = PlayGamesAuthProvider.GetCredential(authCode);
            SignInWithCredential(credential);
        });
    }
#endif

#if (UNITY_IOS || UNITY_TVOS)
public void AuthenticateToGameCenter()
    {
        Social.localUser.Authenticate(success => {
            Debug.Log("Game Center Initialization Complete - Result: " + success);
            if (success)
            {
                SignInWithGameCenterAsync();
            }
        });
    }

    public void SignInWithGameCenterAsync()
    {
        var credentialTask = GameCenterAuthProvider.GetCredentialAsync();
        credentialTask.ContinueWithOnMainThread(task => {
            if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
            {
                Credential credential = task.Result;
                SignInWithCredential(credential);
            }
            else
            {
                Debug.LogError("Game Center sign-in failed: " + task.Exception);
            }
        });
    }
   
#endif

    private void SignInEmail()
    {
        Auth.SignInWithEmailAndPasswordAsync("test2@silverglobegames.com", "Test123@#").ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("Email sign-in failed: " + task.Exception);
                return;
            }

            FirebaseUser user = Auth.CurrentUser;
            if (user != null)
            {
                Debug.Log("Signed in email as: " + user.UserId);

                UserId = "TESTPC" + " (" + user.Email + ")";
                DisplayName = "TESTPC" + " (" + user.Email + ")";
                Email = user.Email;

                long dateValue = long.Parse(user.Metadata.CreationTimestamp.ToString());
                var dateTime = DateTimeOffset.FromUnixTimeMilliseconds(dateValue).UtcDateTime;
                Timestamp timestamp = Timestamp.FromDateTime(dateTime);
                CreatedDate = timestamp;

                FirestoreManager.Instance.GetFirestoreDatas(UserId, true);
            }
        });
    }


    private void SignInWithCredential(Credential credential)
    {
        Auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("Sign in with credential failed: " + task.Exception);
                return;
            }

            FirebaseUser user = Auth.CurrentUser;

            if (user != null)
            {
                Debug.Log("Signed in as: " + user.DisplayName);

                UserId = user.UserId;
                DisplayName = user.DisplayName;
                Email = user.Email;
                PhotoUrl = "ios_null";

                long dateValue = long.Parse(user.Metadata.CreationTimestamp.ToString());
                var dateTime = DateTimeOffset.FromUnixTimeMilliseconds(dateValue).UtcDateTime;
                Timestamp timestamp = Timestamp.FromDateTime(dateTime);
                CreatedDate = timestamp;

                FirestoreManager.Instance.GetFirestoreDatas(user.UserId, true);
            }
        });
    }
}