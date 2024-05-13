using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nuts : MonoBehaviour
{
    [SerializeField] float forceValue = 750f;
    [SerializeField] float decelerationRate = 30f; // Zamanla azalma oran�
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
        // Rigidbody 2D bile�enini al
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
                // E�er Rigidbody 2D bile�eni yoksa veya h�z s�f�rsa, i�lem yapma
                if (rb == null || rb.velocity == Vector2.zero)
                    return;

                // Zamana ba�l� olarak h�z� azalt
                rb.velocity -= decelerationRate * Time.deltaTime * rb.velocity.normalized;

                // E�er h�z s�f�rdan k���kse, s�f�rla
                if (rb.velocity.magnitude < 0.01f)
                {
                    rb.velocity = Vector2.zero;
                    isStartForce = false;
                }
            }
        }        
    }
}
