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
        currentHealth = maxHealth;
    }
    
    public void OnDamage()
    {
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].material = whiteMaterial;
        }
        view.RPC("TakeDamage", RpcTarget.All); // Hasarý senkronize et
        Invoke("InvokeSkin", 0.2f);
    }

    private void InvokeSkin()
    {
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].material = defaultMaterial;
        }
        view.RPC("ChangeSkin", RpcTarget.All);
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
