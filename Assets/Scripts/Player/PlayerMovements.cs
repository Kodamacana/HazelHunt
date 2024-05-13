using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class PlayerMovements : MonoBehaviour
{
    Rigidbody2D rb;
    BoxCollider2D collider;

    private float horizontalInput;
    private float verticalInput;
    [SerializeField] float moveSpeed = 10f;
    Vector2 moveDir;
    PhotonView view;
    Animator anim;

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

    private void FixedUpdate()
    {
        if (view.IsMine && !GetComponent<GunController>().isForceFeedback)
        {
            rb.velocity = moveDir * moveSpeed;
        }        
    }

    private void Update()
    {
        if (view.IsMine)
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
    }
}
