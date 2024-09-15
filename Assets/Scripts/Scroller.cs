using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class Scroller : MonoBehaviour
{
    private RawImage _img;
    [SerializeField] private float _x, _y;

    private void Awake()
    {
        _img = GetComponent<RawImage>();
    }

    void FixedUpdate()
    {
        _img.uvRect = new Rect(_img.uvRect.position + new Vector2(_x, _y) * Time.deltaTime, _img.uvRect.size);
    }
}
