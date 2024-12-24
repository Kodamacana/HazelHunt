using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public static Bomb Instance;
    [SerializeField] private float duration = 1f;
    [SerializeField] private AnimationCurve animCurve;
    [SerializeField] private float heightY = 3f;

    [SerializeField] private float radius = 1.2f;

    PhotonView view;
    Vector3 explosionAreaCoordinate;
    GlobalVolumeAnimator depthOfFieldAnimator;

    private void Awake()
    {
        Instance = this;
        view = GetComponent<PhotonView>();
        depthOfFieldAnimator = GlobalVolumeAnimator.Instance;
    }

    public void ThrowingBomb(Vector3 characterPosition, Vector3 direction)
    {
        explosionAreaCoordinate = direction;
        PoolableObject grapeShadow = PoolManager.Instance.GetObjectFromPool("bombShadow");
        grapeShadow.transform.SetPositionAndRotation(transform.position + new Vector3(0, -0.3f, 0), Quaternion.identity);

        PoolableObject bombRadius = PoolManager.Instance.GetObjectFromPool("bombRadius");
        bombRadius.transform.SetPositionAndRotation(direction, Quaternion.identity);

        StartCoroutine(ProjectileCurveRoutine(characterPosition, direction));

        Vector3 grapeShadowStartPosition = grapeShadow.transform.position;

        StartCoroutine(MoveGrapeShadowRoutine(grapeShadow, bombRadius, grapeShadowStartPosition, direction));
    }

    private IEnumerator ProjectileCurveRoutine(Vector3 startPosition, Vector3 endPosition)
    {
        float timePassed = 0f;

        while (timePassed < duration)
        {
            timePassed += Time.deltaTime;
            float linearT = timePassed / duration;
            float heightT = animCurve.Evaluate(linearT);
            float height = Mathf.Lerp(0f, heightY, heightT);

            transform.position = Vector2.Lerp(startPosition, endPosition, linearT) + new Vector2(0f, height);

            yield return null;
        }
        PoolableObject grapeShadow = PoolManager.Instance.GetObjectFromPool("bombImpact");
        grapeShadow.transform.SetPositionAndRotation(transform.position, Quaternion.identity);

        StartCoroutine(ReturnObject(grapeShadow , 5f));

        view.RPC("ExplosionBomb", RpcTarget.All);
    }

    [PunRPC]
    private void ExplosionBomb()
    {
        depthOfFieldAnimator.StartChromaticAberrationAnimation();
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(explosionAreaCoordinate, radius);
        List<PhotonView> arr = new();

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider != null && hitCollider.name.Contains("Player"))
            {
                PhotonView targetPhotonView = hitCollider.GetComponent<PhotonView>();
                var photonList = arr.Where(x => x.ViewID.Equals(targetPhotonView.ViewID)).ToList();

                if (photonList.Count > 0)
                    continue;

                if (targetPhotonView != null)
                {
                    if(targetPhotonView.IsMine)
                        BombDamageSFX.Instance.StartLowPassAdjustment();

                    targetPhotonView.RPC("TakeDamage", RpcTarget.All, RandomVector2(3.1415f, 3.1415f));

                    arr.Add(targetPhotonView);
                }
            }
        }

        GameController.Instance.ShakeCamera(2);
        Debug.Log("Bomba patlad� ve alan hasar� verildi.");
        GetComponent<SpriteRenderer>().color = Color.clear;
        StartCoroutine(DestroyObject(gameObject,6f));
    }

    public Vector2 RandomVector2(float angle, float angleMin)
    {
        float random = Random.value * angle + angleMin;
        return new Vector2(Mathf.Cos(random), Mathf.Sin(random));
    }

    private IEnumerator MoveGrapeShadowRoutine(PoolableObject grapeShadow, PoolableObject bombRadius, Vector3 startPosition, Vector3 endPosition)
    {
        float timePassed = 0f;

        while (timePassed < duration)
        {
            timePassed += Time.deltaTime;
            float linearT = timePassed / duration;
            grapeShadow.transform.position = Vector2.Lerp(startPosition, endPosition, linearT);
            yield return null;
        }

        grapeShadow.ReturnToPool();
        bombRadius.ReturnToPool();
    }

    private IEnumerator ReturnObject(PoolableObject obj, float time)
    {
        yield return new WaitForSecondsRealtime(time);
        obj.ReturnToPool();
    }


    private IEnumerator DestroyObject(GameObject obj, float time)
    {
        yield return new WaitForSecondsRealtime(time);
        Destroy(obj);
    }
}
