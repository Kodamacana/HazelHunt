using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerManager : MonoBehaviour
{
    [SerializeField] float duration = 180f;

    [SerializeField]  private Image image;
    private PhotonView view;
    private float timer = 0f;
    private bool isGameOver = false;

    private void Awake()
    {
        view = GetComponent<PhotonView>();
    }

    [PunRPC]
    private void Timer(float fillAmount)
    {
        image.fillAmount = fillAmount;
    }

    [PunRPC]
    private void RestartGame()
    {
        //RestartGame
    }

    private void Update()
    {
        if (!isGameOver)
        {
            float fillAmount = 1f;
            if (timer < duration)
            {
                timer += Time.deltaTime; 
                fillAmount = 1f - (timer / duration);
                image.fillAmount = fillAmount;
            }
            else
            {                
                isGameOver = true;
                image.fillAmount = 0f;
            }

            view.RPC("Timer", RpcTarget.AllBufferedViaServer, fillAmount);
        }
        else
        {
            isGameOver = false;
            view.RPC("RestartGame", RpcTarget.All);
        }
    }    
}
