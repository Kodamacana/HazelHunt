using Photon.Pun;
using System.Collections;
using UnityEngine;

public class BaseCollect : MonoBehaviour
{
    NutsCollect nutsCollect;
    GameController gameController;
    PhotonView view;
    GameObject player;

    private void Awake()
    {
        player = GunController.instance.gameObject;
        view = GetComponent<PhotonView>();
    }

    [PunRPC]
    private void UpdateSpriteProperties2()
    {
        transform.parent.transform.localScale = new Vector3 (-0.4892681f, 0.4892681f , 0.4892681f );
        if (view != null && view.IsMine)
        {
            GetComponent<BaseCollect>().nutsCollect = player.GetComponent<NutsCollect>();
            GetComponent<BaseCollect>().gameController = GameController.Instance;
        }  
    }

    [PunRPC]
    private void UpdateSpriteProperties1()
    {
        transform.parent.transform.localScale = new Vector3(0.4892681f, 0.4892681f, 0.4892681f);

        if (view != null && view.IsMine)
        {
            GetComponent<BaseCollect>().nutsCollect = player.GetComponent<NutsCollect>();
            GetComponent<BaseCollect>().gameController = GameController.Instance;
            nutsCollect = player.GetComponent<NutsCollect>();
        } 
    }

    public IEnumerator WaitForObjectCreation(GameObject player)
    {
        while (view == null || player == null)
        {
            yield return null;
        }

        this.player = player;

        if (view.IsMine)
        {
            if (PhotonNetwork.IsMasterClient)               
            {
                view.RPC("UpdateSpriteProperties1", RpcTarget.All);
            }
            else
            {
                view.RPC("UpdateSpriteProperties2", RpcTarget.All);
            }
        }

    }

   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (view.IsMine && collision.name.Contains("Player"))
        {
            bool isCollectNut = collision.GetComponent<NutsCollect>().isCollectNut;
            if (isCollectNut)
            {
                GameController.Instance.UpdateScore();
                SoundManagerSO.PlaySoundFXClip(GameController.Instance.sound_NutCollect, transform.position, 1f);

                collision.GetComponent<PhotonView>().RPC("ResetCollectObj", RpcTarget.AllBuffered);
            }
        }
    }

    
}
