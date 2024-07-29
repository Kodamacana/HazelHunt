using UnityEngine;
using Photon.Pun;
using TMPro;
using System;
using System.Collections;

public class PlayerBase : MonoBehaviourPunCallbacks
{
    [SerializeField] ParticleSystem blood;
    [SerializeField] TextMeshProUGUI usernameText;

    [SerializeField] Sprite[] sprites;
    [SerializeField] SpriteRenderer[] spriteRenderers;
    [SerializeField] Material whiteMaterial;
    [SerializeField] Material defaultMaterial;

    [SerializeField] private int maxHealth = 100;

    public int currentHealth = 100;
    private PhotonView view;

    private void Start()
    {
        view = GetComponent<PhotonView>();
        currentHealth = maxHealth;
        usernameText.text = view.Owner.NickName;
    }

    IEnumerator InvokeSkin()
    {
        GameController.Instance.DamagePlayer();
        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].material = defaultMaterial;
        }
        view.RPC("ChangeSkin", RpcTarget.All);
    }

    [PunRPC]
    public void TakeDamage()
    {
        SoundManagerSO.PlaySoundFXClip(GameController.Instance.sound_GettingShot, transform.position, 1f);

        blood.Play();
        currentHealth -= 35;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
        }

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].material = whiteMaterial;
        }
        StartCoroutine("InvokeSkin");
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
