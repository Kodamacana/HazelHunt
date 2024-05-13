using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using Unity.VisualScripting;
using CodeMonkey.Utils;

public class GunController : MonoBehaviour
{
    public static GunController instance;
    private event EventHandler<OnShootEventArgs> OnShoot;
    private class OnShootEventArgs: EventArgs
    {
        public Vector3 gunEndPointPosition;
        public Vector3 shootPosition;
    }
    public bool isForceFeedback = false;
    [SerializeField] private float bulletSpeed = 100f; // Mermi hýzý
    [SerializeField] private float bulletLifetime = .2f; // Mermi ömrü

    [SerializeField] private RectTransform canvasUsername;
    [SerializeField] private Transform gunEndPointTransform;
    [SerializeField] private Transform gunTransform;
    [SerializeField] private Transform firePoint;

    [SerializeField] private GameObject bulletPrefab; // Mermi önceden hazýrlanmýþ bir GameObject

    private GunController gunController;
    private GameObject player;

    [SerializeField] private float fireCooldownTime = 1.5f; // Cooldown süresi
    private float fireCooldownTimer = 0.0f; // Cooldown zamanlayýcýsý
    bool isFire = false;

    public float recoilForce = 100f;

    GameObject bulletClone;

    [SerializeField] NutsCollect nutsCollect;
    PhotonView view;
    PhotonRigidbody2DView rbView;
    Vector2 lastDirection;

    [SerializeField] GameObject weaponObject;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        view = GetComponent<PhotonView>();
        player = gameObject;
        gunController = GetComponent<GunController>();
        gunController.OnShoot += GunController_OnShoot;
    }
       
    private void HandleAiming()
    {
        if (view.IsMine)
        {
            Vector3 mousePosition = GetMouseWorldPosition();

            Vector3 aimDirection = (mousePosition - transform.position).normalized;
            float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            gunTransform.eulerAngles = new Vector3(0, 0, angle);

            Vector3 localScale = Vector3.one;
            if (angle > 90 || angle < -90)
            {
                localScale.y = -1f;
                player.transform.localScale = new Vector3(1,1,1);
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
            //gunTransform.localScale = localScale;
        }
    }

    private void HandleShooting()
    {
        if (view.IsMine)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePosition = GetMouseWorldPosition();

                //muzzle animator play
                OnShoot?.Invoke(this, new OnShootEventArgs
                {
                    gunEndPointPosition = gunEndPointTransform.position,
                    shootPosition = mousePosition
                });
            }
        }
    }


    private void GunController_OnShoot(object sender, OnShootEventArgs e)
    {
        if (view.IsMine && isFire)
        {            
            Vector3 characterPosition = player.transform.position;
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Fare pozisyonuna doðru bir vektör oluþtur
            Vector2 direction = new Vector2(mousePosition.x - characterPosition.x, mousePosition.y - characterPosition.y);

            // Yön vektörünü normalleþtir
            direction.Normalize();

            // Fare pozisyonuna doðru ateþ etme
            FireBullet(direction);
        }
    }

   
    public void ResetForceFeedback()
    {
        if (view.IsMine)
        {
            isForceFeedback = false;
        }       
    }

    private void ResetNut()
    {
        if (view.IsMine)
        {
            InvokeRepeating("nutsPositioning", 0f, 3f);
            nutsCollect.isCollectNut = false;
            nutsCollect.nutClone = null;
        }
    }

    private void FireBullet(Vector2 direction)
    {  
        if (view.IsMine && nutsCollect.nutClone != null)
        {
            isFire = false;
            fireCooldownTimer = fireCooldownTime;
            if (view.IsMine)
            {
                Rigidbody2D nutRigidbody = nutsCollect.nutClone.transform.GetComponent<Rigidbody2D>();
                nutRigidbody.bodyType = RigidbodyType2D.Dynamic;
                nutRigidbody.simulated = true;

            }

            nutsCollect.nutClone.GetComponent<Nuts>().FireNut(direction);
            Invoke("ResetNut", 0.5f);
        }
        else if(view.IsMine && !isForceFeedback)
        {
            // Açýyý radyan cinsinden hesapla
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Mermiyi oluþtur
            bulletClone = PhotonNetwork.Instantiate(bulletPrefab.name, firePoint.position, Quaternion.AngleAxis(angle, Vector3.forward));

            bulletClone.transform.SetParent(player.transform.GetChild(0).transform);
            bulletClone.transform.localScale = Vector3.one;

            //// Mermiye hýz ver
            //Rigidbody2D rb = bulletClone.GetComponent<Rigidbody2D>();
            //rb.velocity = direction * bulletSpeed;

            isFire = false;
            Invoke("DestroyBullet", bulletLifetime);
            isForceFeedback = true;
            player.GetComponent<Rigidbody2D>().AddForce(-direction * recoilForce, ForceMode2D.Impulse);
            Invoke("ResetForceFeedback", 0.1f);
        }
    }

    private void DestroyBullet()
    {
        PhotonNetwork.Destroy(bulletClone);
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
        view.RPC("ShowWeapon", RpcTarget.AllBuffered);
        HandleAiming();
        HandleShooting();

        fireCooldownTimer -= Time.deltaTime;

        // Eðer cooldown zamanlayýcýsý sýfýrdan küçük veya eþitse, yetenek kullanýlabilir durumdadýr
        if (fireCooldownTimer <= 0.0f)
        {
            isFire = true;
            // Cooldown zamanlayýcýsýný resetle
            fireCooldownTimer = fireCooldownTime;
        }
    }

    [PunRPC]
    public void ShowWeapon()
    {
        if (nutsCollect.isCollectNut && nutsCollect.nutClone != null)
        {
            weaponObject.SetActive(false);
        }

        else
        {
            weaponObject.SetActive(true);
        }
    }

    private void nutsPositioning()
    {
        if (view.IsMine && nutsCollect.isCollectNut && nutsCollect.nutClone != null )
        {
            nutsCollect.nutClone.transform.localPosition = new Vector3(0.52f, 0, 0);
        }
    }
}
