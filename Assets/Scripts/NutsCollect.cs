using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class NutsCollect : MonoBehaviour
{
    [HideInInspector] public bool isCollectNut;
    [HideInInspector] public GameObject nutClone;
    [SerializeField] GameObject nutPrefab;
    [SerializeField] Transform spawnNutParent;

    PhotonView view;

    private void Start()
    {
        view = GetComponent<PhotonView>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.Contains("Nuts(Clone)") && !isCollectNut && nutClone == null)
        {
            Destroy(collision.gameObject);
            view.RPC("CollectNutsOnPool", RpcTarget.AllBuffered);
            
            isCollectNut = true;

        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name.Contains("NutsPool") && !isCollectNut && nutClone == null)
        {
            view.RPC("CollectNutsOnPool", RpcTarget.AllBuffered);

            isCollectNut = true;
        }
    }

    [PunRPC]
    public void CollectNutsOnPool()
    {
        nutClone = Instantiate(nutPrefab, new Vector3(0.52f, 0f, 0f), Quaternion.identity, spawnNutParent);
        nutClone.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        nutClone.GetComponent<Rigidbody2D>().simulated = false;
        GunController.instance.ShowWeapon();
        //nutClone.transform.localPosition = new Vector3(0.52f, 0, 0);
    }
}
