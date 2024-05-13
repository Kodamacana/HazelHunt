using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerManager : MonoBehaviour
{
    PhotonView view;
    Image image; // Image bile�enine referans
    [SerializeField] float duration = 180f; // Animasyon s�resi (saniye)

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
            timer += Time.deltaTime; // Zamanlay�c�y� art�r
            float fillAmount = 1f - (timer / duration); // Filled de�eri hesapla (1'den 0'a do�ru azal�r)
            image.fillAmount = fillAmount; // Image'in filled de�erini ayarla
        }
        else if (!isGameOver && timer >= duration)
        {
            // Oyun bitti�inde
            isGameOver = true;
            image.fillAmount = 0f; // Filled de�erini s�f�rla
                                   //Tur bitti Oyunu ba�tan ba�latma kodu.
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
