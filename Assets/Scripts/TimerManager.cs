using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerManager : MonoBehaviour
{
    PhotonView view;
    Image image; // Image bileþenine referans
    [SerializeField] float duration = 180f; // Animasyon süresi (saniye)

    private float timer = 0f;
    private bool isGameOver = false;

    private void Awake()
    {
        image = GetComponent<Image>();
        view = GetComponent<PhotonView>();
    }

    [PunRPC]
    private void Timer()
    {
        if (!isGameOver && timer < duration)
        {
            timer += Time.deltaTime; // Zamanlayýcýyý artýr
            float fillAmount = 1f - (timer / duration); // Filled deðeri hesapla (1'den 0'a doðru azalýr)
            image.fillAmount = fillAmount; // Image'in filled deðerini ayarla
        }
        else if (!isGameOver && timer >= duration)
        {
            // Oyun bittiðinde
            isGameOver = true;
            image.fillAmount = 0f; // Filled deðerini sýfýrla
                                   //Tur bitti Oyunu baþtan baþlatma kodu.
        }
    }
    private void Update()
    {
        if (!isGameOver)
        {
            view.RPC("Timer", RpcTarget.AllBuffered);
        }
    }

    
}
