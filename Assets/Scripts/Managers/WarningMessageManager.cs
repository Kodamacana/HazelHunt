using System.Collections;
using TMPro;
using UnityEngine;

public class WarningMessageManager : MonoBehaviour
{
    public static WarningMessageManager instance;

    [SerializeField] private TextMeshProUGUI warningTitle;
    [SerializeField] private TextMeshProUGUI warningMessage;
    [SerializeField] private GameObject canvas;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    IEnumerator ClosePanel()
    {
        yield return new WaitForSecondsRealtime(3f);
        canvas.SetActive(false);
    }
    public void SetMessage(string msg, string title = "Warning")
    {
        warningTitle.text = title;
        warningMessage.text = msg;

        canvas.SetActive(true);
        StartCoroutine(ClosePanel());
    }
}
