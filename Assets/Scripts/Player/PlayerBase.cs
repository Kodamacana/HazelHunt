using UnityEngine;
using Photon.Pun;
using TMPro;
using System;
using System.Collections;
using Photon.Realtime;

public class PlayerBase : MonoBehaviourPunCallbacks
{
    [SerializeField] ParticleSystem blood;
    [SerializeField] TextMeshProUGUI usernameText;

    [SerializeField] Camera playerCamera;
    [SerializeField] RenderTexture masterPlayerRendererTexture;
    [SerializeField] RenderTexture guestPlayerRendererTexture;

    [SerializeField] Camera MF_playerCamera;
    [SerializeField] RenderTexture MF_masterPlayerRendererTexture;
    [SerializeField] RenderTexture MF_guestPlayerRendererTexture;

    [SerializeField] Sprite[] sprites;
    [SerializeField] SpriteRenderer[] spriteRenderers;
    [SerializeField] Material whiteMaterial;
    [SerializeField] Material defaultMaterial;


    [SerializeField] private int maxHealth = 100;
    [SerializeField] public int currentHealth = 100;

    private PhotonView view;
    GameController gameController;

    private void Start()
    {
        view = GetComponent<PhotonView>();
        currentHealth = maxHealth;
        gameController = GameController.Instance;

        usernameText.text = view.Owner.NickName;

        if (view.IsMine)
        {
            gameController.masterNickname = view.Owner.NickName;
            playerCamera.targetTexture = masterPlayerRendererTexture;
            MF_playerCamera.targetTexture = MF_masterPlayerRendererTexture;
        }
        else
        {
            gameController.guestNickname = view.Owner.NickName;
            playerCamera.targetTexture = guestPlayerRendererTexture;
            MF_playerCamera.targetTexture = MF_guestPlayerRendererTexture;
        }
        playerCamera.enabled = true;
        MF_playerCamera.enabled = true;
    }

    IEnumerator InvokeSkin()
    {
        gameController.DamagePlayer();
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
            Respawn();
        }

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].material = whiteMaterial;
        }
        StartCoroutine("InvokeSkin");
    }

    private void Respawn()
    {
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

        StartCoroutine(Respawn(2f));
    }

    IEnumerator Respawn(float time)
    {
        currentHealth = 100;
           
        gameController.SpawnPosition(gameObject);
        StartCoroutine(InvokeSkin());

        yield return new WaitForSecondsRealtime(time);
        view.RPC("ResetPlayer", RpcTarget.All);
    }

    [PunRPC]
    private void ResetPlayer()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
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
