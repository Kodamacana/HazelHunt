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
using UnityEngine.Assertions.Must;

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
    [SerializeField] GameObject weaponObject;
    [SerializeField] private GameObject bulletPrefab; // Mermi önceden hazýrlanmýþ bir GameObject
    [SerializeField] NutsCollect nutsCollect;

    [SerializeField] private float fireCooldownTime = 0.5f; // Cooldown süresi
    [SerializeField] private float recoilForce = 3f;
    private float fireCooldownTimer = 0.0f; // Cooldown zamanlayýcýsý

    private PhotonView view;
    private GameObject player;
    public bool isForceFeedback =false;

    private void Awake()
    {
        instance = this;
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

        view.RPC("FireBullet", RpcTarget.AllViaServer, direction);
    }
       


    [PunRPC]
    private void FireBullet(Vector2 direction, PhotonMessageInfo info)
    {
        float lag = (float)(PhotonNetwork.Time - info.SentServerTime);
        fireCooldownTimer = fireCooldownTime;

        if (nutsCollect.nutClone != null)
        {
            nutsCollect.nutClone.GetComponent<Nuts>().FireNut(direction);
            Invoke("ResetNut", 0.5f);
        }
        else
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.AngleAxis(angle, Vector3.forward));
            bullet.GetComponent<BulletForShootgun>().InitializeBullet(direction, lag, angle);
            weaponAnim.Play("Shotgun");

            GetComponent<Rigidbody2D>().AddForce(-direction * recoilForce, ForceMode2D.Impulse);
            Invoke("ResetForceFeedback", 0.1f);
        }        
    }
    public void ResetForceFeedback()
    {
        isForceFeedback = false;
    }


    [PunRPC]
    public void ShowWeapon()
    {
        if (nutsCollect.isCollectNut && nutsCollect.nutClone != null)
            weaponObject.SetActive(false);
        else weaponObject.SetActive(true);
    }


    private void ResetNut()
    {
        if (!view.IsMine)
            return;

        nutsCollect.isCollectNut = false;
        nutsCollect.nutClone = null;
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
