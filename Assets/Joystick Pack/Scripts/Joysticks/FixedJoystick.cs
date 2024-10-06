using UnityEngine;
using UnityEngine.EventSystems;

public class FixedJoystick : Joystick
{
    Animator _animator;

    public override void OnPointerDown(PointerEventData eventData)
    {
        TryGetComponent(out _animator);
        _animator.SetBool("CloseImage", false);
        _animator.SetBool("OpenImage", true);
        base.OnPointerDown(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        TryGetComponent(out _animator);
        _animator.SetBool("OpenImage", false);
        _animator.SetBool("CloseImage", true);
        base.OnPointerUp(eventData);
    }

}