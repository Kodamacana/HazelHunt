using Firebase.Firestore;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class LeaderboardManager : MonoBehaviour
{
    FirebaseFirestore db;

    async void Start()
    {
        db = FirebaseFirestore.DefaultInstance;

        string currentUsername = FirebaseManager.Instance.UserName;
        await GetLeaderboardDataAndRankAsync(currentUsername); // Mevcut kullan�c�n�n UserID'sini g�nder
    }

    // Asenkron liderlik tablosu verilerini �eken ve s�ralamay� bulma fonksiyonu
    async Task GetLeaderboardDataAndRankAsync(string currentUserId)
    {
        try
        {
            CollectionReference usersRef = db.Collection("Users");

            // 'score' alan�na g�re azalan s�rada s�ralama
            QuerySnapshot querySnapshot = await usersRef.OrderByDescending("score").GetSnapshotAsync();

            // S�ralamay� bulma de�i�kenleri
            int rank = 0;
            bool userFound = false;

            // Verileri i�leme
            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                rank++; // Her kullan�c� i�in s�ralamay� art�r

                Dictionary<string, object> userData = documentSnapshot.ToDictionary();

                // �� i�e 'personal_data' alan�na eri�im
                Dictionary<string, object> personalData = (Dictionary<string, object>)userData["personal_data"];
                string userName = personalData["name"].ToString();
                int userScore = int.Parse(userData["score"].ToString());

                // Kullan�c� ad� ve skoru ekrana yazd�r
                Debug.Log($"S�ra: {rank}, Kullan�c� Ad�: {userName}, Skor: {userScore}");

                // E�er s�ralanan kullan�c� mevcut kullan�c�ysa
                if (documentSnapshot.Id == currentUserId)
                {
                    userFound = true;
                    Debug.Log($"Mevcut kullan�c� s�ralamas�: {rank}, Kullan�c� Ad�: {userName}, Skor: {userScore}");
                }
            }

            // E�er kullan�c� bulunamad�ysa
            if (!userFound)
            {
                Debug.Log("Mevcut kullan�c� s�ralamada bulunamad�.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Liderlik tablosu verileri al�n�rken hata olu�tu: " + e);
        }
    }
}
