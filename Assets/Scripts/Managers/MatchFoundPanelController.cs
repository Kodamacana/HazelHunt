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
    [SerializeField] TimerManager timerManager;

    GameController gameController;

    PhotonView view;

    public float delay = 3f;
    private double closeTime;

    private void OnEnable()
    {
        gameController = GameController.Instance;
        view = GetComponent<PhotonView>();

        txtMasterUsername.text = gameController.masterNickname;
        txtGuestUsername.text = gameController.guestNickname;

        closeTime = PhotonNetwork.Time + delay;
        view.RPC("ClosePanel", RpcTarget.AllBufferedViaServer, closeTime);
    }

    [PunRPC]
    void ClosePanel(double closeAt)
    {
        closeTime = closeAt;
        double timeRemaining = closeTime - PhotonNetwork.Time;
        if (timeRemaining > 0)
        {
            StartCoroutine(ClosePanelAfterDelay((float)timeRemaining));
        }
        else gameObject.SetActive(false);
    }

    private IEnumerator ClosePanelAfterDelay(float delay)
    {
        txtMasterUsername.text = gameController.masterNickname;
        txtGuestUsername.text = gameController.guestNickname;
        yield return new WaitForSeconds(delay * 0.5f);
        txtMasterUsername.text = gameController.masterNickname;
        txtGuestUsername.text = gameController.guestNickname;
        yield return new WaitForSeconds(delay * 0.5f);

        gameObject.SetActive(false);
        timerManager.StartGame();
    }
}
