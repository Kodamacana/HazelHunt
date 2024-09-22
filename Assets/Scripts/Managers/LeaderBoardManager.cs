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
        await GetLeaderboardDataAndRankAsync(currentUsername); // Mevcut kullanýcýnýn UserID'sini gönder
    }

    // Asenkron liderlik tablosu verilerini çeken ve sýralamayý bulma fonksiyonu
    async Task GetLeaderboardDataAndRankAsync(string currentUserId)
    {
        try
        {
            CollectionReference usersRef = db.Collection("Users");

            // 'score' alanýna göre azalan sýrada sýralama
            QuerySnapshot querySnapshot = await usersRef.OrderByDescending("score").GetSnapshotAsync();

            // Sýralamayý bulma deðiþkenleri
            int rank = 0;
            bool userFound = false;

            // Verileri iþleme
            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                rank++; // Her kullanýcý için sýralamayý artýr

                Dictionary<string, object> userData = documentSnapshot.ToDictionary();

                // Ýç içe 'personal_data' alanýna eriþim
                Dictionary<string, object> personalData = (Dictionary<string, object>)userData["personal_data"];
                string userName = personalData["name"].ToString();
                int userScore = int.Parse(userData["score"].ToString());

                // Kullanýcý adý ve skoru ekrana yazdýr
                Debug.Log($"Sýra: {rank}, Kullanýcý Adý: {userName}, Skor: {userScore}");

                // Eðer sýralanan kullanýcý mevcut kullanýcýysa
                if (documentSnapshot.Id == currentUserId)
                {
                    userFound = true;
                    Debug.Log($"Mevcut kullanýcý sýralamasý: {rank}, Kullanýcý Adý: {userName}, Skor: {userScore}");
                }
            }

            // Eðer kullanýcý bulunamadýysa
            if (!userFound)
            {
                Debug.Log("Mevcut kullanýcý sýralamada bulunamadý.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Liderlik tablosu verileri alýnýrken hata oluþtu: " + e);
        }
    }
}
