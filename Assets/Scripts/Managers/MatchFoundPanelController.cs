using Photon.Pun;
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
        yield return new WaitForSecondsRealtime(5f);
        gameObject.SetActive(false);
    }
}
