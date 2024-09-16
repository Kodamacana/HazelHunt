using UnityEngine;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using Firebase.Auth;
using UnityEditor;
using Unity.VisualScripting;
using System.Linq;

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
    public string Phone { get; set; }

    [FirestoreProperty]
    public Timestamp Created { get; set; }

    [FirestoreProperty]
    public string PhotoUrl { get; set; }
}

[FirestoreData]
public struct Progress
{
    [FirestoreProperty]
    public int Nut { get; set; }

    [FirestoreProperty]
    public int HighScore { get; set; }

    [FirestoreProperty]
    public int Score { get; set; }
}

public class FirestoreManager : MonoBehaviour
{
    public static FirestoreManager Instance;

    private ListenerRegistration _listenerUserData;

    [SerializeField] private string UserId;
    [SerializeField] private ConnectToServer ConnectToServer;
    private FirebaseFirestore firestore;

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

    private void OnDestroy()
    {
        _listenerUserData.Stop();
    }

    public void GetFirestoreDatas(string UserId = "", bool isLogin = false)
    {
        if (!string.IsNullOrEmpty(UserId))
        {
            this.UserId = UserId;
        }
        firestore = FirebaseFirestore.DefaultInstance;

        firestore.Collection("Users").Document(this.UserId).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DocumentSnapshot snapshot = task.Result;
                if (!snapshot.Exists)
                {
                    CreateNewUser(this.UserId);
                }
                else
                {
                    Dictionary<string, object> personalData = task.Result.GetValue<Dictionary<string, object>>("personal_data");
                    Dictionary<string, object> progressData = task.Result.GetValue<Dictionary<string, object>>("progress");

                    if (isLogin)
                    {
                        GetUserData(personalData);
                        LoadScene();
                    }
                    GetProgressData(progressData);


                    Debug.Log("User document already exists");
                }
            }
            else
            {
                Debug.LogError("Failed to get user document: " + task.Exception);
            }
        });
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
                    { "phone", authManager.Phone},
                    { "photo", authManager.PhotoUrl},
                    { "login_list", new List<object> { Timestamp.FromDateTime(System.DateTime.UtcNow) } }
                }
            },
            { "progress", new Dictionary<string, object>
                {
                    { "nut", 0 },
                    { "high_score", 0 },
                    { "score", 0 }
                }
            }            
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

    private void GetUserData(Dictionary<string, object> personalData)
    {
        AuthManager authManager = AuthManager.Instance;
        FirebaseUser user = authManager.Auth.CurrentUser;

        UserData userData = new()
        {
            UserName = personalData["name"].ToString(),
            Mail = personalData["mail"].ToString(),
            Phone = personalData["phone"].ToString(),
            LoginList = personalData["login_list"] as List<object>,
            PhotoUrl = personalData["photo"].ToString(),
            Created = new Timestamp()
        };

        FirebaseManager.Instance.SaveLocalFromFirestoreInUserDatas(UserId, userData.UserName, userData.Mail, userData.Phone, userData.LoginList, userData.PhotoUrl);

        userData.LoginList.Add(Timestamp.FromDateTime(DateTime.UtcNow));
        personalData["created"] = FirebaseManager.Instance.UserRegisterDate;
        personalData["login_list"] = userData.LoginList;

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
            HighScore = (int)(long)progressData["high_score"]
        };

        FirebaseManager.Instance.SaveLocalFromFirestoreInProgressDatas(progress.Nut, progress.HighScore, progress.Score);
    }
    
    public void SetNut(int value)
    {
        progress.Nut += value;
        Dictionary<string, object> updatedCoin = new()
        {
            { "progress.coin", progress.Nut}
        };

        firestore.Collection("Users").Document(UserId).UpdateAsync(updatedCoin);
    }
    
    public void SetScore(int value)
    {
        progress.Score = value;
        Dictionary<string, object> updatedScore = new()
        {
            { "progress.score", progress.Score }
        };
        firestore.Collection("Users").Document(UserId).UpdateAsync(updatedScore);
    }

    public void SetHighScore(int value)
    {
        progress.HighScore = value;
        Dictionary<string, object> updatedHighScore = new()
        {
            { "progress.high_score", progress.HighScore }
        };
        firestore.Collection("Users").Document(UserId).UpdateAsync(updatedHighScore);
    }

    //public void UpgradePowerUp(int upgradeablePowerUpId)
    //{
    //    DocumentReference docRef = firestore.Collection("Users").Document(UserId);
    //    DocumentReference upgradeableItemsRef = firestore.Collection("UpgradeableItems").Document("Item_id");

    //    docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
    //    {
    //        if (task.IsCompleted && task.Result.Exists)
    //        {
    //            Dictionary<string, object> powerupUpgradeLevel = task.Result.GetValue<Dictionary<string, object>>("powerup_upgrade_level");
    //            Dictionary<string, object> userProgress = task.Result.GetValue<Dictionary<string, object>>("progress");

    //            int userCoin = Convert.ToInt32(userProgress["coin"]);
    //            int itemLevel = Convert.ToInt32(powerupUpgradeLevel[upgradeablePowerUpId.ToString()]);

    //            upgradeableItemsRef.GetSnapshotAsync().ContinueWithOnMainThread(innerTask =>
    //            {
    //                if (innerTask.IsCompleted && innerTask.Result.Exists)
    //                {
    //                    List<object> itemPricesObj = innerTask.Result.GetValue<List<object>>("Item_Price");
    //                    List<int> itemPrices = itemPricesObj.Select(price => Convert.ToInt32(price)).ToList();

    //                    int itemCost = itemPrices[itemLevel];

    //                    if (userCoin >= itemCost)
    //                    {
    //                        int newUserCoin = userCoin - itemCost;
    //                        int newItemLevel = itemLevel + 1;

    //                        userProgress["coin"] = newUserCoin;
    //                        powerupUpgradeLevel[upgradeablePowerUpId.ToString()] = newItemLevel;

    //                        Dictionary<string, object> updates = new Dictionary<string, object>
    //                    {
    //                        { "progress", userProgress },
    //                        { "powerup_upgrade_level", powerupUpgradeLevel }
    //                    };

    //                        docRef.UpdateAsync(updates).ContinueWithOnMainThread(updateTask =>
    //                        {
    //                            if (updateTask.IsCompleted)
    //                            {
    //                                Debug.Log("User progress and power-up levels successfully updated!");

    //                                progress.Nut = newUserNut;
    //                                FirebaseManager.Instance.SaveLocalFromFirestoreInProgressDatas(
    //                                    progress.Nut, progress.HighScore, progress.Score);

    //                                FirebaseManager.Instance.SaveLocalFromFirestoreInPowerupDatas(
    //                                    Convert.ToInt32(powerupUpgradeLevel["100"]),
    //                                    Convert.ToInt32(powerupUpgradeLevel["101"]),
    //                                    Convert.ToInt32(powerupUpgradeLevel["102"]),
    //                                    Convert.ToInt32(powerupUpgradeLevel["103"]),
    //                                    true);

    //                                FirebaseManager.Instance.PowerupLevelIds[upgradeablePowerUpId] = newItemLevel;
    //                            }
    //                            else
    //                            {
    //                                Debug.LogError("Failed to update document: " + updateTask.Exception);
    //                            }
    //                        });
    //                    }
    //                    else
    //                    {
    //                        GameManager.Instance.OpentoNeedMoneyPopUp();
    //                    }
    //                }
    //                else
    //                {
    //                    Debug.LogError("Error retrieving upgradeable item document: " + innerTask.Exception);
    //                }
    //            });
    //        }
    //        else
    //        {
    //            Debug.LogError("Error retrieving user document: " + task.Exception);
    //        }
    //    });
    //}

    public void LoadScene()
    {
        ConnectToServer.ConnectToTheServer();
        //SceneManager.LoadSceneAsync(1);
    }
}
