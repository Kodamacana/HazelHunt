using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class TimerManager : MonoBehaviourPun
{
    [SerializeField, Range(0, 600)] float duration = 180f;
    [SerializeField] private Image image;
    [SerializeField] EndGamePanel endGamePanel;
    [SerializeField] GameObject onGamePanel;
    [SerializeField] GlobalVolumeAnimator depthOfFieldAnimator;

    private PhotonView view;
    private double startTime;
    [HideInInspector] public bool isGameOver = false;

    private void Awake()
    {
        view = GetComponent<PhotonView>();
    }

    public void StartGame()
    {
        isGameOver = false;
        startTime = PhotonNetwork.Time;
        view.RPC("SyncStartTime", RpcTarget.AllBufferedViaServer, startTime);
    }

    [PunRPC]
    private void SyncStartTime(double syncedStartTime)
    {
        startTime = syncedStartTime;
        isGameOver = false;
    }

    private void Update()
    {
        if (!isGameOver)
        {
            double elapsedTime = PhotonNetwork.Time - startTime;
            if (elapsedTime < duration)
            {
                float fillAmount = 1f - (float)(elapsedTime / duration);
                image.fillAmount = fillAmount;
            }
            else
            {
                isGameOver = true;
                image.fillAmount = 0f;
                view.RPC("FinishGame", RpcTarget.AllBufferedViaServer);
            }
        }
    }

    [PunRPC]
    private void FinishGame()
    {
        endGamePanel.gameObject.SetActive(true);
        onGamePanel.SetActive(false);
        depthOfFieldAnimator.StartDepthOfFieldAnimation();
    }
}
