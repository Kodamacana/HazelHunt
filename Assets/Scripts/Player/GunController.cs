using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Timeline;

public class GunController : MonoBehaviour
{
    public static GunController instance;    

    [SerializeField] private float bulletSpeed = 100f; // Mermi hýzý
    [SerializeField] private float bulletLifetime = .2f; // Mermi ömrü

    [SerializeField] private RectTransform canvasUsername;
    [SerializeField] private Transform gunEndPointTransform;
    [SerializeField] private Transform gunTransform;
    [SerializeField] private Transform firePoint;

    [SerializeField] Animator weaponAnim;
    [SerializeField] GameObject nutPrefab;
    [SerializeField] private GameObject bulletPrefab; // Mermi önceden hazýrlanmýþ bir GameObject
    [SerializeField] NutsCollect nutsCollect;

    [SerializeField] private float fireCooldownTime = 0.5f; // Cooldown süresi
    [SerializeField] private float recoilForce = 3f;
    private float fireCooldownTimer = 0.0f; // Cooldown zamanlayýcýsý
    private PlayerMovements playerMovement;

    private PhotonView view;
    private GameObject player;

    private void Awake()
    {
        instance = this;
        playerMovement = GetComponent<PlayerMovements>();
    }

    void Start()
    {
        view = GetComponent<PhotonView>();
        player = gameObject;
    }

    private void HandleAiming()
    {
        if (!view.IsMine)
            return;

        Vector3 mousePosition = GetMouseWorldPosition();

        Vector3 aimDirection = (mousePosition - transform.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        gunTransform.eulerAngles = new Vector3(0, 0, angle);

        Vector3 localScale = Vector3.one;
        if (angle > 90 || angle < -90)
        {
            localScale.y = -1f;
            player.transform.localScale = new Vector3(1, 1, 1);
            gunTransform.localScale = new Vector3(1, -1, 1);
            canvasUsername.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            localScale.y = +1f;
            player.transform.localScale = new Vector3(-1, 1, 1);
            gunTransform.localScale = new Vector3(-1, 1, 1);
            canvasUsername.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void GunController_OnShoot()
    {
        if (!view.AmOwner )
        {
            return;
        }

        if (view.CreatorActorNr != PhotonNetwork.LocalPlayer.ActorNumber)
        {
            return;
        }

        Vector3 characterPosition = player.transform.position;
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = new Vector2(mousePosition.x - characterPosition.x, mousePosition.y - characterPosition.y);
        direction.Normalize();

        if (nutsCollect.isCollectNut)
        {
            playerMovement.isTrueForceFeedback = true;
            StartCoroutine("ResetForceFeedback");
            fireCooldownTimer = fireCooldownTime;

            view.RPC("ShotNut", RpcTarget.AllBuffered, 1, direction);
        }
        else
        {
            view.RPC("FireBullet", RpcTarget.AllBuffered, direction);
        }
    }

    [PunRPC]
    private void FireBullet(Vector2 direction, PhotonMessageInfo info)
    {
        playerMovement.isTrueForceFeedback = true;
        float lag = (float)(PhotonNetwork.Time - info.SentServerTime);
        fireCooldownTimer = fireCooldownTime;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        RaycastHit2D hitInfo = Physics2D.BoxCast((Vector2)transform.position + direction, new Vector2(1f, 1f), angle, direction, 1f);
        if (hitInfo.collider != null && hitInfo.collider.name.Contains("Player"))
        {
            PhotonView targetPhotonView = hitInfo.transform.GetComponent<PhotonView>();

            if (targetPhotonView != null && !targetPhotonView.IsMine)
            {
                if (view.IsMine)
                {
                    targetPhotonView.RPC("TakeDamage", RpcTarget.All);
                }
            }
        }
        else
        {
            Debug.Log("Çarpýþma yok.");
        }

        #region bullet
        Vector2 pdir;
        Vector2 playerVelocity = GetComponent<Rigidbody2D>().velocity;


        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.AngleAxis(angle, Vector3.forward));
        pdir = Vector2.Perpendicular(direction) * -.01f;
        bullet.GetComponent<BulletForShootgun>().InitializeBullet(direction, lag, angle, pdir, playerVelocity);

        bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.AngleAxis(angle, Vector3.forward));
        pdir = Vector2.Perpendicular(direction) * .001f;
        bullet.GetComponent<BulletForShootgun>().InitializeBullet(direction, lag, angle, pdir, playerVelocity);

        bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.AngleAxis(angle, Vector3.forward));
        pdir = Vector2.Perpendicular(direction) * .01f;
        bullet.GetComponent<BulletForShootgun>().InitializeBullet(direction, lag, angle, pdir, playerVelocity);

        weaponAnim.Play("Shotgun");
        GetComponent<Rigidbody2D>().AddForce(-direction * recoilForce, ForceMode2D.Impulse);
        StartCoroutine("ResetForceFeedback");
        #endregion
    }
      
    private IEnumerator ResetForceFeedback()
    {
        yield return new WaitForSeconds(0.1f);

        playerMovement.isTrueForceFeedback = false;
    }

    [PunRPC]
    private void ShotNut(int a, Vector2 direction)
    {
        if (a == 0)
        {
            nutsCollect.weaponObject.SetActive(false);
            nutsCollect.nutsInventory.SetActive(true);
        }
        else
        {
            nutsCollect.weaponObject.SetActive(true);
            nutsCollect.nutsInventory.SetActive(false);
        }

        if (!view.IsMine)
            return;

        GameObject nutClone = PhotonNetwork.Instantiate(nutPrefab.name, Vector3.zero, Quaternion.identity);
        nutClone.transform.localPosition = transform.position;
        nutClone.GetComponent<Rigidbody2D>().velocity = direction * 5f;

        StartCoroutine( ResetNut());
    }

       
    IEnumerator ResetNut()
    {
        yield return new WaitForSeconds(0.1f);
            
        nutsCollect.isCollectNut = false;
    }

    private static Vector3 GetMouseWorldPosition()
    {
        return GetMouseWorldPositionZ(Input.mousePosition, Camera.main);
    }

    private static Vector3 GetMouseWorldPositionZ(Vector3 screenPosition, Camera worldCamera)
    {
        // Correctly calculate world position with given screen position and camera
        Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
        worldPosition.z = 0f; // Assuming your game is 2D, so z position should be 0
        return worldPosition;
    }
   
    private void Update()
    {  
        HandleAiming();

        if (Input.GetMouseButtonDown(0) && fireCooldownTimer <= 0.0f)
        {
            GunController_OnShoot();
        }

        fireCooldownTimer -= Time.deltaTime;
    }   
}
