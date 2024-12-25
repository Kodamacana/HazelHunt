using Photon.Pun;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NutsCollect : MonoBehaviour
{
    [HideInInspector] public bool isCollectNut;

    [SerializeField] public GameObject nutsInventory;
    [SerializeField] public GameObject weaponObject;
    private PlayerMovements movements;
    private Animator anim;

    PhotonView view;
    private void Awake()
    {
        view = GetComponent<PhotonView>();
        movements = GetComponent<PlayerMovements>();
        anim = GetComponent<Animator>();
    }

    public void DestroyNut()
    {
        view.RPC("CollectNutsOnPool", RpcTarget.AllBufferedViaServer);
        view.RPC("ShowAndHideWeapon", RpcTarget.AllBufferedViaServer);
        anim.SetBool("Run", true);
    }
   
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!view.IsMine)
            return;

        if (collision.gameObject.name.Contains("NutsPool") && !isCollectNut)
        {
            SoundManagerSO.PlaySoundFXClip(GameController.Instance.sound_NutCollect, transform.position, 0.6f);

            view.RPC("CollectNutsOnPool", RpcTarget.AllBufferedViaServer);
            view.RPC("ShowAndHideWeapon", RpcTarget.AllBufferedViaServer);
            anim.SetBool("Run", true);
        }
    }

    [PunRPC]
    private void ShowAndHideWeapon()
    {
        weaponObject.transform.parent.gameObject.SetActive(false);
        movements.moveSpeed = 2.7f;
        //weaponObject.SetActive(false);
        //nutsInventory.SetActive(true);       
    }

    [PunRPC]
    private void CollectNutsOnPool()
    {
        if (!isCollectNut) isCollectNut = true;
    }

    [PunRPC]
    public void ResetCollectObj()
    {
        isCollectNut = false;
        weaponObject.transform.parent.gameObject.SetActive(true);
        weaponObject.SetActive(true);
        nutsInventory.SetActive(false);

        anim.SetBool("Run", false);
        movements.moveSpeed = 2.7f;
    }
}
