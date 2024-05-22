using Photon.Pun;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NutsCollect : MonoBehaviour
{
    [HideInInspector] public bool isCollectNut;

    [SerializeField] public GameObject nutsInventory;
    [SerializeField] public GameObject weaponObject;

    PhotonView view;
    private void Awake()
    {
        view = GetComponent<PhotonView>();
    }

    public void DestroyNut()
    {
        view.RPC("CollectNutsOnPool", RpcTarget.AllBufferedViaServer);
        view.RPC("ShowAndHideWeapon", RpcTarget.AllBufferedViaServer);
    }
   
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!view.IsMine)
            return;

        if (collision.gameObject.name.Contains("NutsPool") && !isCollectNut)
        {
            view.RPC("CollectNutsOnPool", RpcTarget.AllBufferedViaServer);
            view.RPC("ShowAndHideWeapon", RpcTarget.AllBufferedViaServer);
        }
    }

    [PunRPC]
    private void ShowAndHideWeapon()
    {
        weaponObject.SetActive(false);
        nutsInventory.SetActive(true);       
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
        weaponObject.SetActive(true);
        nutsInventory.SetActive(false);
    }
}
