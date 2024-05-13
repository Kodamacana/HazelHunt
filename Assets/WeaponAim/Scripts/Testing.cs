
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class Testing : MonoBehaviour {

    [SerializeField] private PlayerAimWeapon playerAimWeapon;

    private void Start() {
        playerAimWeapon.OnShoot += PlayerAimWeapon_OnShoot;
    }

    private void PlayerAimWeapon_OnShoot(object sender, PlayerAimWeapon.OnShootEventArgs e) {
        UtilsClass.ShakeCamera(.6f, .05f);
        WeaponTracer.Create(e.gunEndPointPosition, e.shootPosition);
        Shoot_Flash.AddFlash(e.gunEndPointPosition);

        Vector3 shootDir = (e.shootPosition - e.gunEndPointPosition).normalized;
        shootDir = UtilsClass.ApplyRotationToVector(shootDir, 90f);
        ShellParticleSystemHandler.Instance.SpawnShell(e.shellPosition, shootDir);
    }
}
