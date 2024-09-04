using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatchFoundPanelController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI txtMasterUsername;
    [SerializeField] TextMeshProUGUI txtGuestUsername;

    [SerializeField] RawImage guestPlayerRawImg;
    [SerializeField] RawImage masterPlayerRawImg;
    [SerializeField] TimerManager timerManager;

    GameController gameController;

    private void OnEnable()
    {
        gameController = GameController.Instance;

        txtMasterUsername.text = gameController.masterNickname;
        txtGuestUsername.text = gameController.guestNickname;

        StartCoroutine(ClosePanel());
    }

    IEnumerator ClosePanel()
    {
        yield return new WaitForSecondsRealtime(1f);
        txtMasterUsername.text = gameController.masterNickname;
        txtGuestUsername.text = gameController.guestNickname;
        yield return new WaitForSecondsRealtime(1f);
        txtMasterUsername.text = gameController.masterNickname;
        txtGuestUsername.text = gameController.guestNickname;
        yield return new WaitForSecondsRealtime(1f);
        txtMasterUsername.text = gameController.masterNickname;
        txtGuestUsername.text = gameController.guestNickname;
        yield return new WaitForSecondsRealtime(1f);
        txtMasterUsername.text = gameController.masterNickname;
        txtGuestUsername.text = gameController.guestNickname;
        yield return new WaitForSecondsRealtime(1f);
        txtMasterUsername.text = gameController.masterNickname;
        txtGuestUsername.text = gameController.guestNickname;
        timerManager.isGameOver = false;
        gameObject.SetActive(false);
    }
}
