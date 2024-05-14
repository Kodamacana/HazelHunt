using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WeaponDamage : MonoBehaviour
{    
    PhotonView view;
    Animator anim;

    private void Awake()
    {
        view = transform.parent.GetComponent<PhotonView>();
        anim = GetComponent<Animator>();
    }

   
    private void Update()
    {
        if (!view.IsMine)
            return;

        if (Input.GetMouseButtonDown(0))
            anim.SetBool("Shot", true);
        else anim.SetBool("Shot", false);
    }
   
}
