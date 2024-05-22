using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class PlayerMovements : MonoBehaviour, IPunObservable
{
    [SerializeField] float moveSpeed = 10f;
    [HideInInspector] public bool isTrueForceFeedback = false;

    private Rigidbody2D rb;
    private float horizontalInput;
    private float verticalInput;
    private Vector2 moveDir;
    private PhotonView view;
    private Animator anim;
    private Vector2 networkPosition;

    private void Start()
    {
        view = GetComponent<PhotonView>();
        anim = GetComponent<Animator>();
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void FixedUpdate()
    {
        if (!view.IsMine)
        {
            transform.position = Vector3.MoveTowards(transform.position, networkPosition, Time.fixedDeltaTime * 100);
            return;
        }
        else if (!isTrueForceFeedback)
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
            stream.SendNext(isTrueForceFeedback);
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            isTrueForceFeedback = (bool)stream.ReceiveNext();
        }
    }
}
