using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;

public class RealDateTimeManager : MonoBehaviour
{
    public static RealDateTimeManager Instance { get; private set; }

    private const string API_URL = "http://worldtimeapi.org/api/ip";
    private DateTime currentDateTime;

    public void GetCurrentDateTime(Action<DateTime> onDateTimeReceived)
    {
        StartCoroutine(InitializeDateTime((dateTime) =>
        {
            onDateTimeReceived?.Invoke(dateTime);
        }));
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private IEnumerator InitializeDateTime(Action<DateTime> onCompleted)
    {
        int retryCount = 0;
        int maxRetries = 50; // En fazla 5 kez dene
        float retryDelay = 0.5f; // Her deneme arasýnda 2 saniye bekle

        while (retryCount < maxRetries)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(API_URL))
            {
                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    string jsonResponse = webRequest.downloadHandler.text;
                    WorldTimeApiResponse response = JsonUtility.FromJson<WorldTimeApiResponse>(jsonResponse);

                    if (DateTime.TryParse(response.datetime, out currentDateTime))
                    {
                        Debug.Log("Current DateTime: " + currentDateTime);
                        onCompleted?.Invoke(currentDateTime);
                    }
                    else
                    {
                        Debug.LogError("Failed to parse datetime."); 
                    }
                }
                else
                {
                    Debug.LogError($"Attempt {retryCount + 1} failed: {webRequest.error}");
                }
            }

            retryCount++;
            if (retryCount < maxRetries)
            {
                yield return new WaitForSeconds(retryDelay); // Gecikme süresi
            }
            else
            {
                Debug.LogError("Max retries reached. Using system time as fallback.");
                onCompleted?.Invoke(DateTime.UtcNow);
            }
        }
    }

    [Serializable]
    private class WorldTimeApiResponse
    {
        public string datetime;
    }
}
