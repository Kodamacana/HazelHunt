using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class NutsCollect : MonoBehaviour
{
    [HideInInspector] public bool isCollectNut;
    [SerializeField] GameObject nutPrefab;
    [SerializeField] Transform spawnNutParent;
     public GameObject nutClone;

    PhotonView view;

    private void Start()
    {
        view = GetComponent<PhotonView>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.Contains("Nuts(Clone)") && !isCollectNut && nutClone == null)
        {
            if (view.IsMine)
            {
                nutClone = collision.gameObject;
                nutClone.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                nutClone.GetComponent<Rigidbody2D>().simulated = false;
                nutClone.transform.SetParent(spawnNutParent);
                StartCoroutine(nutClone.GetComponent<Nuts>().PositionNut());
                isCollectNut = true;
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name.Contains("NutsPool") && !isCollectNut && nutClone == null)
        { 
            view.RPC("CollectNutsOnPool", RpcTarget.AllBuffered);
            if (view.IsMine)
            {
                isCollectNut = true;
                nutClone.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                nutClone.GetComponent<Rigidbody2D>().simulated = false;
            }
        }
    }

    [PunRPC]
    public void CollectNutsOnPool()
    {
        nutClone = PhotonNetwork.Instantiate(nutPrefab.name, new Vector3(0f, 0, 0), Quaternion.identity);

        nutClone.transform.localPosition = new Vector3(0.52f, 0, 0);
        nutClone.SetActive(true);
        nutClone.transform.SetParent(spawnNutParent);
    }
}
