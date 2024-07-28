using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Timeline;
using UnityEngine.TextCore.Text;
using UnityEngine.EventSystems;

public class GunController : MonoBehaviour
{
    public static GunController instance;    

    [SerializeField] private Transform playerHead;
    [SerializeField] private RectTransform canvasUsername;
    [SerializeField] private Transform gunEndPointTransform;
    [SerializeField] private Transform gunTransform;
    [SerializeField] private Transform firePoint;

    [SerializeField] Animator weaponAnim;
    [SerializeField] GameObject nutPrefab;
    [SerializeField] GameObject bombPrefab;
    [SerializeField] private GameObject bulletPrefab; // Mermi önceden hazýrlanmýþ bir GameObject
    [SerializeField] NutsCollect nutsCollect;

    [SerializeField] private float fireCooldownTime = 0.5f; // Cooldown süresi
    [SerializeField] private float bombCooldownTime = 5f; // Cooldown süresi
    [SerializeField] private float recoilForce = 3f;

    private float fireCooldownTimer = 0.0f; // Cooldown zamanlayýcýsý
    private float bombCooldownTimer = 0.0f; // Cooldown zamanlayýcýsý
    private PlayerMovements playerMovement;

    private PhotonView view;
    private GameObject player;
    private Joystick attackJoystick;
    private Joystick bombJoystick;
    private Vector3 lastAttackDirection;

    PoolManager poolManager;
    private void Awake()
    {
        instance = this;
        poolManager = PoolManager.Instance;
        playerMovement = GetComponent<PlayerMovements>();
    }

    void Start()
    {
        view = GetComponent<PhotonView>();
        player = gameObject;

        attackJoystick = GameController.Instance.attackJoystick;
        EventTrigger evTrig1 = attackJoystick.GetComponent<EventTrigger>();

        EventTrigger.Entry clickEvent1 = new EventTrigger.Entry()
        {
            eventID = EventTriggerType.PointerUp
        };
        clickEvent1.callback.AddListener(GunController_OnShoot);
        evTrig1.triggers.Add(clickEvent1);

        bombJoystick = GameController.Instance.bombJoystick;
        EventTrigger evTrig =  bombJoystick.GetComponent<EventTrigger>();

        EventTrigger.Entry clickEvent = new EventTrigger.Entry()
        {
            eventID = EventTriggerType.PointerUp
        };
        clickEvent.callback.AddListener(BombController_OnShoot);
        evTrig.triggers.Add(clickEvent);
    }

    private void HandleAiming()
    {
        if (!view.IsMine)
            return;


        if (attackJoystick.Direction.y != 0)
        {
            Vector3 aimDirection1 = attackJoystick.Direction.normalized; 
            float angle1 = Mathf.Atan2(aimDirection1.y, aimDirection1.x) * Mathf.Rad2Deg;
            gunTransform.eulerAngles = new Vector3(0, 0, angle1);
            playerHead.eulerAngles = new Vector3(0, 0, angle1);
            Vector3 localScale1 = Vector3.one;
            if (angle1 > 90 || angle1 < -90)
            {
                localScale1.y = -1f;
                player.transform.localScale = new Vector3(1, 1, 1);
                gunTransform.localScale = new Vector3(1, -1, 1);
                playerHead.localScale = new Vector3(-1, -1, 1);
                canvasUsername.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                localScale1.y = +1f;
                player.transform.localScale = new Vector3(-1, 1, 1);
                gunTransform.localScale = new Vector3(-1, 1, 1);
                playerHead.localScale = new Vector3(1, 1, 1);
                canvasUsername.localScale = new Vector3(-1, 1, 1);
            }
            lastAttackDirection = aimDirection1;
        }
        else if (bombJoystick.Direction.y != 0)
        {
            Vector3 aimDirection1 = bombJoystick.Direction.normalized;
            float angle1 = Mathf.Atan2(aimDirection1.y, aimDirection1.x) * Mathf.Rad2Deg;
            gunTransform.eulerAngles = new Vector3(0, 0, angle1);
            playerHead.eulerAngles = new Vector3(0, 0, angle1);
            Vector3 localScale1 = Vector3.one;
            if (angle1 > 90 || angle1 < -90)
            {
                localScale1.y = -1f;
                player.transform.localScale = new Vector3(1, 1, 1);
                gunTransform.localScale = new Vector3(1, -1, 1);
                playerHead.localScale = new Vector3(-1, -1, 1);
                canvasUsername.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                localScale1.y = +1f;
                player.transform.localScale = new Vector3(-1, 1, 1);
                gunTransform.localScale = new Vector3(-1, 1, 1);
                playerHead.localScale = new Vector3(1, 1, 1);
                canvasUsername.localScale = new Vector3(-1, 1, 1);
            }
            lastAttackDirection = aimDirection1;
        }


        //Vector3 mousePosition = GetMouseWorldPosition();

        //Vector3 aimDirection = (mousePosition - transform.position).normalized;
        //float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        //gunTransform.eulerAngles = new Vector3(0, 0, angle);
        //playerHead.eulerAngles = new Vector3(0, 0, angle);
        //Vector3 localScale = Vector3.one;
        //if (angle > 90 || angle < -90)
        //{
        //    localScale.y = -1f;
        //    player.transform.localScale = new Vector3(1, 1, 1);
        //    gunTransform.localScale = new Vector3(1, -1, 1);
        //    playerHead.localScale = new Vector3(-1, -1, 1);
        //    canvasUsername.localScale = new Vector3(1, 1, 1);
        //}
        //else
        //{
        //    localScale.y = +1f;
        //    player.transform.localScale = new Vector3(-1, 1, 1);
        //    gunTransform.localScale = new Vector3(-1, 1, 1);
        //    playerHead.localScale = new Vector3(1, 1, 1);
        //    canvasUsername.localScale = new Vector3(-1, 1, 1);
        //}
    }

    public void GunController_OnShoot(BaseEventData eventData)
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
        //Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Vector2 direction = new Vector2(mousePosition.x - characterPosition.x, mousePosition.y - characterPosition.y);
        //direction.Normalize();

        Vector2 direction = lastAttackDirection;
        direction.Normalize();

        if (nutsCollect.isCollectNut)
        {
            playerMovement.isTrueForceFeedback = true;
            StartCoroutine("ResetForceFeedback");
            fireCooldownTimer = fireCooldownTime;

            view.RPC("ThrowNut", RpcTarget.AllBuffered, 1, direction);
        }
        else
        {
            view.RPC("FireBullet", RpcTarget.AllBuffered, direction);
        }
    }

    public void BombController_OnShoot(BaseEventData eventData)
    {
        if (!view.AmOwner)
        {
            return;
        }

        if (view.CreatorActorNr != PhotonNetwork.LocalPlayer.ActorNumber)
        {
            return;
        }

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        PhotonNetwork.Instantiate("Bomb", transform.position, Quaternion.identity);

        view.RPC("ThrowBomb", RpcTarget.AllBuffered, mousePosition, transform.position);
    }

    [PunRPC]
    private void ThrowBomb(Vector3 mousePosition, Vector3 bombPosition)
    {
        bombCooldownTimer = bombCooldownTime;
        Vector2 characterPosition = transform.position;

        //Vector2 direction = new Vector2(mousePosition.x - characterPosition.x, mousePosition.y - characterPosition.y);

        Vector2 direction = new Vector2(lastAttackDirection.x - characterPosition.x, lastAttackDirection.y - characterPosition.y);

        Vector2 clampedOffset = Vector2.ClampMagnitude(direction, 3f);
        Vector2 newDirection = characterPosition + clampedOffset;
        direction = newDirection;

        Bomb.Instance.ThrowingBomb(characterPosition, direction );

        SoundManagerSO.PlaySoundFXClip(GameController.Instance.sound_Bomb, bombPosition, 1f);
    }

    [PunRPC]
    private void FireBullet(Vector2 direction)
    {
        playerMovement.isTrueForceFeedback = true;
        fireCooldownTimer = fireCooldownTime;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Sabit mesafe ve açý için
        float offsetAngle = 5f; // Açý offseti (derece cinsinden)
        float distance = 1.5f; // Ray mesafesi

        // Aþaðý ve yukarý kaydýrýlmýþ hedef noktalarý
        Vector2[] directions = new Vector2[5];
        directions[0] = direction; // Merkez yön
        directions[1] = Quaternion.Euler(0, 0, -offsetAngle) * direction; // 1. Aþaðý kaydýrýlmýþ yön
        directions[2] = Quaternion.Euler(0, 0, -2 * offsetAngle) * direction; // 2. Aþaðý kaydýrýlmýþ yön
        directions[3] = Quaternion.Euler(0, 0, offsetAngle) * direction; // 1. Yukarý kaydýrýlmýþ yön
        directions[4] = Quaternion.Euler(0, 0, 2 * offsetAngle) * direction; // 2. Yukarý kaydýrýlmýþ yön

        // Raycast kontrolü
        foreach (var dir in directions)
        {
            dir.Normalize();
            RaycastHit2D hitInfo = Physics2D.BoxCast((Vector2)transform.position + dir, new Vector2(0.5f, 0.5f), angle, dir, distance);
            if (hitInfo.collider != null && hitInfo.collider.name.Contains("Player"))
            {
                PhotonView targetPhotonView = hitInfo.transform.GetComponent<PhotonView>();
                if (targetPhotonView != null && !targetPhotonView.IsMine)
                {
                    if (view.IsMine)
                    {
                        SoundManagerSO.PlaySoundFXClip(GameController.Instance.sound_GettingShot, hitInfo.collider.transform.position, 1f);
                        targetPhotonView.RPC("TakeDamage", RpcTarget.All);
                        break;
                    }
                }
            }
            else
            {
                Debug.Log("Çarpýþma yok.");
            }
        }

        #region bullet

        Vector2 newDirection = new(direction.x * 5, direction.y * 5);

        Vector2 lowerDirection = Quaternion.Euler(0, 0, -5) * newDirection;
        Vector2 upperDirection = Quaternion.Euler(0, 0, 5) * newDirection;

        Vector2 playerVelocity = GetComponent<Rigidbody2D>().velocity;

        PoolableObject bullet = poolManager.GetObjectFromPool("bullet");
        bullet.transform.SetPositionAndRotation(firePoint.position, Quaternion.AngleAxis(angle, Vector3.forward));
        bullet.GetComponent<BulletForShootgun>().InitializeBullet(newDirection, playerVelocity);
        StartCoroutine(ReturnPool(bullet));

        PoolableObject bullet2 = poolManager.GetObjectFromPool("bullet");
        bullet2.transform.SetPositionAndRotation(firePoint.position, Quaternion.AngleAxis(angle, Vector3.forward));
        bullet2.GetComponent<BulletForShootgun>().InitializeBullet(lowerDirection, playerVelocity);
        StartCoroutine(ReturnPool(bullet2));

        PoolableObject bullet3 = poolManager.GetObjectFromPool("bullet");
        bullet3.transform.SetPositionAndRotation(firePoint.position, Quaternion.AngleAxis(angle, Vector3.forward));
        bullet3.GetComponent<BulletForShootgun>().InitializeBullet(upperDirection, playerVelocity);
        StartCoroutine(ReturnPool(bullet3));

        weaponAnim.Play("Shotgun");
        SoundManagerSO.PlaySoundFXClip(GameController.Instance.sound_Shotgun, player.transform.position, 1f);
        GetComponent<Rigidbody2D>().AddForce(-direction * recoilForce, ForceMode2D.Impulse);
        StartCoroutine("ResetForceFeedback");
        #endregion
    }

    private IEnumerator ReturnPool(PoolableObject obj)
    {
        yield return new WaitForSeconds(0.2f);
        obj.ReturnToPool();
    }


    private IEnumerator ResetForceFeedback()
    {
        yield return new WaitForSeconds(0.1f);

        playerMovement.isTrueForceFeedback = false;
    }

    [PunRPC]
    private void ThrowNut(int a, Vector2 direction)
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
           // GunController_OnShoot();
        }
        if (Input.GetMouseButtonDown(1) && bombCooldownTimer <= 0.0f)
        {
           // BombController_OnShoot();
        }

        fireCooldownTimer -= Time.deltaTime;
        bombCooldownTimer -= Time.deltaTime;

        Vector3 characterPosition = player.transform.position;
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 direction = new(mousePosition.x - characterPosition.x, mousePosition.y - characterPosition.y);
        direction.Normalize();

        float distance = 3f; // BoxCast mesafesi
        float offsetAngle = 5f; // Açý offseti (derece cinsinden)
        Vector2 boxSize = new Vector2(0.5f, 0.5f); // BoxCast kutu boyutu

        // Merkez BoxCast
        if (Physics2D.BoxCast(transform.position, boxSize, 0, transform.TransformDirection(direction), distance))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(direction) * distance, Color.red);
        }

        // 1. Aþaðý kaydýrýlmýþ BoxCast
        Vector2 lowerDirection1 = Quaternion.Euler(0, 0, -offsetAngle) * direction;
        lowerDirection1.Normalize();
        if (Physics2D.BoxCast(transform.position, boxSize, 0, transform.TransformDirection(lowerDirection1), distance))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(lowerDirection1) * distance, Color.blue);
        }

        // 2. Aþaðý kaydýrýlmýþ BoxCast
        Vector2 lowerDirection2 = Quaternion.Euler(0, 0, -2 * offsetAngle) * direction;
        lowerDirection2.Normalize();
        if (Physics2D.BoxCast(transform.position, boxSize, 0, transform.TransformDirection(lowerDirection2), distance))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(lowerDirection2) * distance, Color.cyan);
        }

        // 1. Yukarý kaydýrýlmýþ BoxCast
        Vector2 upperDirection1 = Quaternion.Euler(0, 0, offsetAngle) * direction;
        upperDirection1.Normalize();
        if (Physics2D.BoxCast(transform.position, boxSize, 0, transform.TransformDirection(upperDirection1), distance))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(upperDirection1) * distance, Color.green);
        }

        // 2. Yukarý kaydýrýlmýþ BoxCast
        Vector2 upperDirection2 = Quaternion.Euler(0, 0, 2 * offsetAngle) * direction;
        upperDirection2.Normalize();
        if (Physics2D.BoxCast(transform.position, boxSize, 0, transform.TransformDirection(upperDirection2), distance))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(upperDirection2) * distance, Color.yellow);
        }

    }

}
