using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nuts : MonoBehaviour
{
    [SerializeField] float forceValue = 750f;
    [SerializeField] float decelerationRate = 30f; // Zamanla azalma oraný
    bool isStartForce = false;

    Rigidbody2D rb;
    PhotonView view;
    Vector2 lastDirection;

    private void Start()
    {
        view = GetComponent<PhotonView>();
        StartCoroutine(PositionNut());
    }

    private void Awake()
    {
        // Rigidbody 2D bileþenini al
        rb = GetComponent<Rigidbody2D>();
    }
         
    public IEnumerator PositionNut()
    {
        while (transform.parent == null)
        {
            yield return null;
        }
        transform.localPosition = new Vector3(0.52f, 0, 0);
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        GetComponent<Rigidbody2D>().simulated = false;
    }

    private void StartMovement(Vector2 direction)
    {
        isStartForce = true;

        rb = transform.GetComponent<Rigidbody2D>();
        rb.AddForce(direction * forceValue);
    }

    public void FireNut(Vector2 direction)
    {
        lastDirection = direction;
        view.RPC("OnShootNut", RpcTarget.AllBuffered);
        
        StartMovement(lastDirection);
    }

    [PunRPC]
    public void OnShootNut()
    {
        transform.SetParent(null);       
    }

    private void Update()
    {
        if (view.IsMine)
        {
            if (isStartForce)
            {
                // Eðer Rigidbody 2D bileþeni yoksa veya hýz sýfýrsa, iþlem yapma
                if (rb == null || rb.velocity == Vector2.zero)
                    return;

                // Zamana baðlý olarak hýzý azalt
                rb.velocity -= decelerationRate * Time.deltaTime * rb.velocity.normalized;

                // Eðer hýz sýfýrdan küçükse, sýfýrla
                if (rb.velocity.magnitude < 0.01f)
                {
                    rb.velocity = Vector2.zero;
                    isStartForce = false;
                }
            }
        }        
    }
}
