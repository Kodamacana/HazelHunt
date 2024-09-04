using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerManager : MonoBehaviour
{
    [SerializeField, Range(0,600)] float duration = 180f;

    [SerializeField]  private Image image;
    [SerializeField] EndGamePanel endGamePanel;

    private PhotonView view;
    private float timer = 0f;
    [HideInInspector] public bool isGameOver = true;

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
    private void FinishGame()
    {
        endGamePanel.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (!isGameOver)
        {
            float fillAmount;
            if (timer < duration)
            {
                timer += Time.deltaTime; 
                fillAmount = 1f - (timer / duration);
                image.fillAmount = fillAmount;
                view.RPC("Timer", RpcTarget.AllBuffered, fillAmount);
            }
            else
            {                
                isGameOver = true;
                image.fillAmount = 0f;
                view.RPC("FinishGame", RpcTarget.AllBuffered);
            }            
        }
    }    
}
