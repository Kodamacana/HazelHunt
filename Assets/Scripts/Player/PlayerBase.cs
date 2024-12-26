using UnityEngine;
using Photon.Pun;
using TMPro;
using System;
using System.Collections;
using Photon.Realtime;
using Unity.Cinemachine;

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

    private KillPlayer killPlayer;

    private PhotonView view;
    GameController gameController;

    private void Start()
    {
        view = GetComponent<PhotonView>();
        killPlayer = GetComponent<KillPlayer>();
        currentHealth = maxHealth;
        gameController = GameController.Instance;

        usernameText.text = view.Owner.NickName;

        if (view.IsMine)
        {
            gameObject.tag = "Player";
            gameController.masterNickname = view.Owner.NickName;
            playerCamera.targetTexture = masterPlayerRendererTexture;
            MF_playerCamera.targetTexture = MF_masterPlayerRendererTexture;
        }
        else
        {
            gameObject.tag = "Enemy";
            gameController.guestNickname = view.Owner.NickName;
            playerCamera.targetTexture = guestPlayerRendererTexture;
            MF_playerCamera.targetTexture = MF_guestPlayerRendererTexture;
        }
        playerCamera.enabled = true;
        MF_playerCamera.enabled = true;

        CinemachineTargetGroup.Target target = new CinemachineTargetGroup.Target(); 
        target.Object = transform;
        gameController.targetGroup.Targets.Add(target);
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
    public void TakeDamage(Vector2 direction, bool isBomb)
    {

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        blood.transform.eulerAngles = new Vector3(0, 0, angle);
        blood.Play();

        if (isBomb)
        {
            SoundManagerSO.PlaySoundFXClip(GameController.Instance.sound_GettingShot, transform.position, 0.2f);
            SoundManagerSO.PlaySoundFXClip(GameController.Instance.sound_ImpactSplat1, transform.position, 0.05f);
            currentHealth -= 70;
        }
        else {
            SoundManagerSO.PlaySoundFXClip(GameController.Instance.sound_GettingShot, transform.position, 0.2f);
            SoundManagerSO.PlaySoundFXClip(GameController.Instance.sound_ImpactSplat2, transform.position, .05f);
            currentHealth -= 8;
        }

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            killPlayer.KilledThePlayer(direction, transform, view.IsMine);
            Respawn();
        }
        gameController.ShakeCamera(3);

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].material = whiteMaterial;
        }
        StartCoroutine(nameof(InvokeSkin));

        if(view.IsMine)
            gameController.bloodOverlayAnimator.SetTrigger("Blood");
    }

    private void Respawn()
    {
        for (int i = 0; i < transform.childCount-1; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        GetComponent<PlayerMovements>().enabled = false;
        GetComponent<GunController>().enabled = false;
        GetComponent<NutsCollect>().enabled = false;
        GetComponent<CapsuleCollider2D>().enabled = false;

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
        GetComponent<PlayerMovements>().enabled = true;
        GetComponent<GunController>().enabled = true;
        GetComponent<NutsCollect>().enabled = true;
        GetComponent<CapsuleCollider2D>().enabled = true;
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
