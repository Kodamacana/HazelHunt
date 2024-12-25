using UnityEngine;
using Photon.Pun;

public class BulletPhysics : MonoBehaviourPun
{
    public float speed = 10f; 
    public float range = 15f; 
    private Vector2 startPosition;
    public Vector2 direction;

    void Start()
    {
        startPosition = transform.position;
    }

    void FixedUpdate()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);

        float distanceTravelled = Vector2.Distance(startPosition, transform.position);

        if (distanceTravelled >= range)
        {
            if (photonView.IsMine)
                PhotonNetwork.Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.tag.Contains("Obstacle") && !collision.tag.Contains("Enemy"))
            return;

        if (!photonView.IsMine)
            return;

        if (collision.tag.Contains("Enemy"))
        {
            PhotonView targetPhotonView = collision.transform.GetComponent<PhotonView>();
            if (targetPhotonView != null && !targetPhotonView.IsMine)
            {
                targetPhotonView.RPC("TakeDamage", RpcTarget.All, direction, false);
            }
        }
        else PhotonNetwork.Destroy(gameObject);
    }
}
