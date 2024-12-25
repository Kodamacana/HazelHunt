using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.EventSystems;

public class GunController : MonoBehaviour
{
    #region Fields
    public static GunController instance;

    [SerializeField] private Transform playerHead;
    [SerializeField] private RectTransform canvasUsername;
    [SerializeField] private Transform gunTransform;
    [SerializeField] private Transform squirrelHead;
    [SerializeField] public Transform firePoint;
    [SerializeField] public GameObject bulletDirectionIndicator;

    private Animator playerAnim;
    [SerializeField] private GameObject nutPrefab;
    [SerializeField] private GameObject bombCrosshairPrefab;
    [SerializeField] private NutsCollect nutsCollect;

    [SerializeField, Range(0,10)] private float fireCooldownTime = 0.5f; // Cooldown s resi
    [SerializeField, Range(0, 10)] private float bombCooldownTime = 5f; // Cooldown s resi
    [SerializeField, Range(0, 10)] private float recoilForce = 3f;

    private float fireCooldownTimer = 0.0f; // Cooldown zamanlay c s 
    private float bombCooldownTimer = 0.0f; // Cooldown zamanlay c s 
    private Vector3 lastAttackDirection;

    private PlayerMovements playerMovement;
    private PhotonView view;
    private GameObject player;
    private Joystick attackJoystick;
    private Joystick bombJoystick;
    private GameObject bombCrosshair;

    private PoolManager poolManager;
    #endregion


    #region InitializeGame
    private void Awake()
    {
        InitializeSingleton();
        InitializeComponents();
        InstantiateCrosshair();
    }

    private void Start()
    {
        InitializePhotonView();

        if (!view.IsMine)
            return;

        InitializePlayer();
        InitializeJoysticks();
        SetupJoystickEvents();
    }

    private void InitializeSingleton()
    {
        instance = this;
    }

    private void InitializeComponents()
    {
        poolManager = PoolManager.Instance;
        playerMovement = GetComponent<PlayerMovements>();
        playerAnim = GetComponent<Animator>();
    }

    private void InstantiateCrosshair()
    {
        bombCrosshair = Instantiate(bombCrosshairPrefab);
    }

    private void InitializePhotonView()
    {
        view = GetComponent<PhotonView>();
    }

    private void InitializePlayer()
    {
        player = gameObject;
    }

    private void InitializeJoysticks()
    {
        attackJoystick = GameController.Instance.attackJoystick;
        bombJoystick = GameController.Instance.bombJoystick;
    }

    private void SetupJoystickEvents()
    {
        SetupJoystickEvent(attackJoystick, GunController_OnShoot);
        SetupJoystickEvent(bombJoystick, BombController_OnShoot);
    }

    private void SetupJoystickEvent(Joystick joystick, UnityEngine.Events.UnityAction action)
    {
        EventTrigger eventTrigger = joystick.GetComponent<EventTrigger>();

        // Mevcut t m eventleri temizle
        eventTrigger.triggers.Clear();

        // Yeni event ekle
        EventTrigger.Entry clickEvent = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerUp
        };
        clickEvent.callback.AddListener((eventData) => action.Invoke());

        eventTrigger.triggers.Add(clickEvent);
    }
    #endregion


    #region AimHandle
    private void HandleAiming()
    {
        if (attackJoystick.Direction.y != 0)
        {
            HandleJoystickAiming(attackJoystick.Direction.normalized);
        }
        else
        {
            squirrelHead.localScale = Vector3.one;
            bulletDirectionIndicator.SetActive(false);
        }
        
        if (bombJoystick.Direction.y != 0)
        {
            HandleBombAiming(bombJoystick.Direction);
        }
    }

    private void HandleJoystickAiming(Vector3 aimDirection)
    {
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        RotatePlayerAndGun(angle);
        SetPlayerAndGunScale(angle);
        lastAttackDirection = aimDirection;

        bulletDirectionIndicator.SetActive(true);
    }

    private void HandleBombAiming(Vector3 aimDirection)
    {
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        Vector2 newBombCrosshairPosition = (Vector2)player.transform.position + (Vector2)aimDirection * 3f;
        bombCrosshair.transform.position = new Vector3(newBombCrosshairPosition.x, newBombCrosshairPosition.y, bombCrosshair.transform.position.z);
        bombCrosshair.SetActive(true);

        RotatePlayerAndGun(angle);
        SetPlayerAndGunScale(angle);
        lastAttackDirection = bombCrosshair.transform.position;
    }

    private void RotatePlayerAndGun(float angle)
    {
        gunTransform.eulerAngles = new Vector3(0, 0, angle);
        playerHead.eulerAngles = new Vector3(0, 0, angle);
    }

    private void SetPlayerAndGunScale(float angle)
    {
        Vector3 localScale = Vector3.one;
        if (angle > 90 || angle < -90)
        {
            localScale.y = -1f;
            //player.transform.localScale = new Vector3(1, 1, 1);
            gunTransform.localScale = new Vector3(1, -1, 1);
            squirrelHead.localScale = playerMovement.squirrelBody.transform.localScale.x <= 0 ? new Vector3(-1, 1, 1) : new Vector3(1, 1, 1);

            //canvasUsername.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            localScale.y = 1f;
           // player.transform.localScale = new Vector3(-1, 1, 1);
            gunTransform.localScale = new Vector3(1, 1, 1);
            squirrelHead.localScale = playerMovement.squirrelBody.transform.localScale.x <= 0 ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);

            //canvasUsername.localScale = new Vector3(-1, 1, 1);
        }
    }
    #endregion


    #region Events
    public void GunController_OnShoot()
    {
        if (fireCooldownTimer > 0.0f)
            return;

        if (!view.AmOwner)
            return;

        if (view.CreatorActorNr != PhotonNetwork.LocalPlayer.ActorNumber)
            return;

        //Vector3 characterPosition = player.transform.position;
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

            view.RPC("ThrowNut", RpcTarget.AllBuffered, direction);
        }
        else
        {
            view.RPC("FireBullet", RpcTarget.AllBuffered, direction);
            GameController.Instance.ShakeCamera(0);
        }
    }

    public void BombController_OnShoot()
    {
        bombCrosshair.SetActive(false);

        if (bombCooldownTimer > 0.0f)
            return;

        if (!view.AmOwner)
            return;

        if (view.CreatorActorNr != PhotonNetwork.LocalPlayer.ActorNumber)
            return;

        // Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        PhotonNetwork.Instantiate("Bomb", transform.position, Quaternion.identity);

        view.RPC("ThrowBomb", RpcTarget.AllBuffered, lastAttackDirection);
    }

    #endregion


    #region PunRPCs

    [PunRPC]
    private void ThrowBomb(Vector3 mousePosition)
    {
        bombCooldownTimer = bombCooldownTime;
        Vector2 characterPosition = transform.position;

        //Vector2 direction = new Vector2(mousePosition.x - characterPosition.x, mousePosition.y - characterPosition.y);

        Vector2 direction = new Vector2(mousePosition.x - characterPosition.x, mousePosition.y - characterPosition.y);

        Vector2 clampedOffset = Vector2.ClampMagnitude(direction, 3f);
        Vector2 newDirection = characterPosition + clampedOffset;
        direction = newDirection;

        Bomb.Instance.ThrowingBomb(characterPosition, direction);

        SoundManagerSO.PlaySoundFXClip(GameController.Instance.sound_Bomb, direction, 0.6f);
    }


    public int pelletsCount = 5; // Sa�ma say�s�
    public float spreadAngle = 30f; // Sa�malar�n yay�lma a��s�
    public float bulletForce = 10f; // Mermi h�z�


    [PunRPC]
    private void FireBullet(Vector2 direction)
    {
        playerMovement.isTrueForceFeedback = true;
        fireCooldownTimer = fireCooldownTime;

        for (int i = 0; i < pelletsCount; i++)
        {
            GameObject bullet = PhotonNetwork.Instantiate("Bullet", firePoint.position, firePoint.rotation);

            bullet.GetComponent<BulletPhysics>().direction = direction;
            float randomAngle = Random.Range(-spreadAngle / 2, spreadAngle / 2);
            Quaternion rotation = Quaternion.Euler(0, 0, firePoint.rotation.eulerAngles.z + randomAngle);

            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

            float randomForce = Random.Range(bulletForce * 0.4f, bulletForce);
            rb.AddForce(rotation * Vector2.right * randomForce, ForceMode2D.Impulse);
        }

        playerAnim.Play("ShotgunPlayer");
        PoolableObject bulletCase = poolManager.GetObjectFromPool("bulletcase");
        bulletCase.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
        StartCoroutine(ReturnBulletCasePool(bulletCase));

        SoundManagerSO.PlaySoundFXClip(GameController.Instance.sound_Shotgun, transform.position, 0.8f);
        StartCoroutine("ResetForceFeedback");
        GetComponent<Rigidbody2D>().AddForce(-direction * recoilForce, ForceMode2D.Impulse);
    }

    [PunRPC]
    private void ThrowNut(Vector2 direction)
    {
        nutsCollect.weaponObject.SetActive(true);
        nutsCollect.nutsInventory.SetActive(false);
        nutsCollect.weaponObject.transform.parent.gameObject.SetActive(true);
        playerMovement.moveSpeed = 2.2f;
        playerMovement.anim.SetBool("Run", false);

        if (!view.IsMine)
            return;

        GameObject nutClone = PhotonNetwork.Instantiate(nutPrefab.name, Vector3.zero, Quaternion.identity);
        nutClone.transform.localPosition = transform.position;
        nutClone.GetComponent<Rigidbody2D>().linearVelocity = direction * 5f;

        StartCoroutine(ResetNut());
    }
    #endregion


    #region IEnumertors

    IEnumerator ResetNut()
    {
        yield return new WaitForSeconds(0.1f);

        nutsCollect.isCollectNut = false;
    }

    IEnumerator ResetForceFeedback()
    {
        yield return new WaitForSeconds(0.1f);

        playerMovement.isTrueForceFeedback = false;
    }

    IEnumerator ReturnPool(PoolableObject obj)
    {
        yield return new WaitForSeconds(0.2f);
        obj.ReturnToPool();
    }

    IEnumerator ReturnBulletCasePool(PoolableObject obj)
    {
        yield return new WaitForSeconds(10f);
        obj.ReturnToPool();
    }

    #endregion


    private void Update()
    {
        fireCooldownTimer -= Time.deltaTime;
        bombCooldownTimer -= Time.deltaTime;

        if (!view.IsMine)
            return;
        HandleAiming();
    }
}
