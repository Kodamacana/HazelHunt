using UnityEngine;
using Photon.Pun;

public class BulletPhysics : MonoBehaviourPun
{
    public float speed = 10f; 
    public float range = 15f; 
    private Vector2 startPosition; 

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);

        float distanceTravelled = Vector2.Distance(startPosition, transform.position);

        if (distanceTravelled >= range)
        {
            if (photonView.IsMine)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Contains("CameraBounds") || collision.name.Contains("Bullet"))
            return;

        if (collision.name.Contains("Player"))
        {
            PhotonView targetPhotonView = collision.transform.GetComponent<PhotonView>();
            if (targetPhotonView != null && !targetPhotonView.IsMine)
            {
                if (photonView.IsMine)
                {
                    targetPhotonView.RPC("TakeDamage", RpcTarget.All, transform.rotation);
                }
            }
        }

        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
