using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class PlayerMovements : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] public float moveSpeed = 2.2f;
    [SerializeField] public RectTransform squirrelBody;
    [SerializeField] public Transform squirrelWeapon;
    private Vector3 refSquirrelBodyScale;
    [HideInInspector] public bool isTrueForceFeedback = false;

    private Rigidbody2D rb;
    private float horizontalInput;
    private float verticalInput;
    private Vector2 moveDir;
    private PhotonView view;
    [HideInInspector]public Animator anim;
    private Vector2 networkPosition;
    [HideInInspector] public Joystick movementJoystick;
    [HideInInspector] public Joystick attackJoystick;
    [SerializeField] ParticleSystem dustParticle;
    [SerializeField] ParticleSystem grassParticle;

    public float acceleration = 15f;  // Ývme, hýzlanma ve yavaþlamanýn ne kadar hýzlý olacaðýný belirler
    public float deacceleration = 10f;

    private void Start()
    {
        view = GetComponent<PhotonView>();
        anim = GetComponent<Animator>();
        refSquirrelBodyScale = squirrelBody.localScale;
        movementJoystick = GameController.Instance.movementJoystick;
        attackJoystick = GameController.Instance.attackJoystick;
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
            //rb.linearVelocity = moveDir * moveSpeed;

            transform.Translate(moveDir * Time.deltaTime);
        }
    }

    private void Update()
    {
        if (!view.IsMine)
        {
            return;
        }

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector2 inputDirection = new Vector2(moveX, moveY).normalized;

        if (movementJoystick.Direction.y != 0)
        {
            if (movementJoystick.Direction.x <= 0)
            {
                squirrelBody.localScale = refSquirrelBodyScale;

                if (attackJoystick.Direction.y == 0)
                {
                    squirrelWeapon.localRotation = Quaternion.Euler(0, 0, 180f);
                    squirrelWeapon.localScale = new(1, -1, 1);
                }
            }
            else
            {
                squirrelBody.localScale = new Vector3(-refSquirrelBodyScale.x, refSquirrelBodyScale.y, 1);

                if (attackJoystick.Direction.y == 0)
                {
                    squirrelWeapon.localRotation = Quaternion.Euler(0, 0, 0f);
                    squirrelWeapon.localScale = new(1, 1, 1);
                }
            }

            inputDirection = new Vector2(movementJoystick.Direction.x, movementJoystick.Direction.y).normalized;

            anim.SetBool("Walk", true);
            grassParticle.Play();
            //dustParticle.Play();
        }
        else
        {
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
                squirrelBody.localScale = refSquirrelBodyScale;
            }
            if (Input.GetKey(KeyCode.D))
            {
                horizontalInput = 1f;
                squirrelBody.localScale = new Vector3(-refSquirrelBodyScale.x, refSquirrelBodyScale.y, 1);
            }
            if (horizontalInput == 0 && verticalInput == 0)
            {
                anim.SetBool("Walk", false);
            }
            else
            {
                anim.SetBool("Walk", true);
               // dustParticle.Play();
                grassParticle.Play();
            }
            
            //moveDir = new Vector2(horizontalInput, verticalInput).normalized;                   
        }

        if (inputDirection.magnitude > 0)
        {
            moveDir = Vector2.MoveTowards(moveDir, inputDirection * moveSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            moveDir = Vector2.MoveTowards(moveDir, Vector2.zero, deacceleration * Time.deltaTime);
        }
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
