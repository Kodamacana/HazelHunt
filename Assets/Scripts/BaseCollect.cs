using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class BaseCollect : MonoBehaviour
{
    public NutsCollect nutsCollect;
    GameController gameController;
    PhotonView view;

    bool isFlip;
    GameObject player;

    private void Awake()
    {
        player = GunController.instance.gameObject;
        view = GetComponent<PhotonView>();
    }

    [PunRPC]
    public void UpdateSpriteProperties2()
    {
        transform.parent.transform.localScale = new Vector3 (-0.4892681f, 0.4892681f , 0.4892681f );
        if (view != null && view.IsMine)
        {
            GetComponent<BaseCollect>().nutsCollect = player.GetComponent<NutsCollect>();
            GetComponent<BaseCollect>().gameController = GameController.Instance;
        }  
    }

    [PunRPC]
    public void UpdateSpriteProperties1()
    {
        transform.parent.transform.localScale = new Vector3(0.4892681f, 0.4892681f, 0.4892681f);

        if (view != null && view.IsMine)
        {
            GetComponent<BaseCollect>().nutsCollect = player.GetComponent<NutsCollect>();
            GetComponent<BaseCollect>().gameController = GameController.Instance;
            nutsCollect = player.GetComponent<NutsCollect>();
        } 
    }

    public IEnumerator WaitForObjectCreation(GameObject player, bool isFlipX)
    {
        while (view == null || player == null)
        {
            yield return null;
        }

        isFlip = isFlipX;
        this.player = player;

        if (view.IsMine)
        {
            int playerNumber = GetPlayerNumber();
            if (playerNumber == 1)               
            {
                view.RPC("UpdateSpriteProperties1", RpcTarget.AllBuffered, null);
            }
            else if(playerNumber == 2)
            {
                view.RPC("UpdateSpriteProperties2", RpcTarget.AllBuffered, null);
            }
        }

    }
    private int GetPlayerNumber()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == 2)
        {
            return 1;
        }
        else if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
        {
            return 2;
        }
        else
        {
            Debug.LogError("Invalid player number!");
            return -1;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (view.IsMine && collision.name.Contains("Player"))
        {
            bool isCollectNut = collision.GetComponent<NutsCollect>().isCollectNut;
            if (gameController.chosenTree.Equals(transform.parent.gameObject) && isCollectNut)
            {
                GameController.Instance.UpdateScore();
                PhotonNetwork.Destroy(collision.GetComponent<NutsCollect>().nutClone);
                nutsCollect.isCollectNut = false;
            }
        }
    }
}
