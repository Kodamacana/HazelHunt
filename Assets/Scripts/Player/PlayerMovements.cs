using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class PlayerMovements : MonoBehaviour, IPunObservable
{
    Rigidbody2D rb;
    BoxCollider2D collider;

    private float horizontalInput;
    private float verticalInput;
    [SerializeField] float moveSpeed = 10f;
    Vector2 moveDir;
    PhotonView view;
    Animator anim;
    private Vector2 networkPosition;

    private void Start()
    {
        view = GetComponent<PhotonView>();
        anim = GetComponent<Animator>();
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
    }

    public void FixedUpdate()
    {
        if (!view.IsMine)
        {
            rb.position = Vector3.MoveTowards(rb.position, networkPosition, Time.fixedDeltaTime * 100);
            return;
        }
        else if (!GetComponent<GunController>().isForceFeedback)
        {
            rb.velocity = moveDir * moveSpeed;
        }
    }

    private void Update()
    {
        if (!view.IsMine)
        {
            return;
        }   

        horizontalInput = 0;
        verticalInput = 0;

        if (Input.GetKey(KeyCode.W))
        {
            verticalInput = 1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            verticalInput = -1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            horizontalInput = -1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            horizontalInput = 1f;
        }

        if (horizontalInput == 0 && verticalInput == 0)
        {
            anim.SetBool("Run", false);
        }
        else
        {
            anim.SetBool("Run", true);
        }
        moveDir = new Vector2(horizontalInput, verticalInput).normalized;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();

            float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
            networkPosition += (rb.velocity * lag);
        }
    }
}
