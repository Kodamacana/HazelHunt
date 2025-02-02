using Photon.Pun;
using System.Collections;
using UnityEngine;

public class Nuts : MonoBehaviourPunCallbacks
{
    [SerializeField] float damping = 0.1f; // Azalma katsay s 

    Rigidbody2D rb;
    PhotonView view;
    bool isOut = false;

    private void Awake()
    {
        SoundManagerSO.PlaySoundFXClip(GameController.Instance.sound_NutCollect, transform.position, 0.6f);

        rb = GetComponent<Rigidbody2D>();
        view = GetComponent<PhotonView>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    { 
        if (collision.CompareTag("Player") && isOut)
        {
            if (!collision.GetComponent<PhotonView>().IsMine || collision.GetComponent<NutsCollect>().isCollectNut)
                return;

            isOut = false;
            collision.GetComponent<NutsCollect>().DestroyNut();

            view.RPC("DestroyNutObj", RpcTarget.AllBufferedViaServer);
        }
        isOut = true;
    }

    [PunRPC]
    private void DestroyNutObj()
    {
        if (view.IsMine || PhotonNetwork.IsMasterClient)
        {
            SoundManagerSO.PlaySoundFXClip(GameController.Instance.sound_NutCollect, transform.position, 0.6f);

            PhotonNetwork.Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity *= (1 - damping);
    }
}
