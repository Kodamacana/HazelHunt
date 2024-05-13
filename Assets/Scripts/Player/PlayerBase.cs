using UnityEngine;
using Photon.Pun;
using TMPro;
using System;

public class PlayerBase : MonoBehaviour
{
    //[SerializeField] TextMeshProUGUI usernameText;
    [SerializeField]
    Sprite[] sprites;

    [SerializeField]
    SpriteRenderer[] spriteRenderers;

    [SerializeField] private int maxHealth = 100;
    public int currentHealth;
    private PhotonView view;

    [SerializeField] Material whiteMaterial;
    [SerializeField] Material defaultMaterial;

    private void Start()
    {
        view = GetComponent<PhotonView>();
        //if (view.IsMine)
        //{
        //    usernameText.text = PhotonNetwork.NickName;
        //}
        currentHealth = maxHealth;
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (view != null)
        {
            if (!view.IsMine) // Eðer sahibi ben deðilsem
                return;

            if (collision.transform.name.Contains("Bullet")) // Eðer çarpýþan obje bir mermi ise
            {
                for (int i = 0; i < spriteRenderers.Length; i++)
                {
                    spriteRenderers[i].material = whiteMaterial;
                }
                view.RPC("TakeDamage", RpcTarget.All); // Hasarý senkronize et
                Invoke("InvokeSkin", 0.2f);
            }

        }
    }

    private void InvokeSkin()
    {
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].material = defaultMaterial;
        }
    }

    [PunRPC]
    public void TakeDamage()
    {
        currentHealth -= 35;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
        }
    }


    [PunRPC]
    public void ChangeSkin()
    {

        if (currentHealth > 0 && currentHealth < 35)
        {
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                spriteRenderers[i].sprite = sprites[i + (2 * spriteRenderers.Length)];
            }
        }
        else if (currentHealth > 34 && currentHealth < 70)
        {
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                spriteRenderers[i].sprite = sprites[i + spriteRenderers.Length];
            }
        }
        else if (currentHealth > 69 && currentHealth < 101)
        {
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                spriteRenderers[i].sprite = sprites[i];
            }
        }

    }
    private void Update()
    {
        if (view.IsMine)
        {
            view.RPC("ChangeSkin", RpcTarget.All);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(currentHealth);
        }
        else
        {
            currentHealth = (int)stream.ReceiveNext();
        }
    }
}
