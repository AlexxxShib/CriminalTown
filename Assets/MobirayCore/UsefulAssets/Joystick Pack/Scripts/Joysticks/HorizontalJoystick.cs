using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class HorizontalJoystick : FloatingJoystick
{
    public override float Vertical => verticalValue;

    [Space]
    public float UpSpeed = 1;
    public float DowndSpeed = 1;

    [Space]
    public float VerticalMaxValue = 1;
    public float VerticalMinValue = 0;

    public float verticalValue = 0;
    public float direction = -1;
    public float onPointerDown = 0;

    public Action<bool> OnPointerDownListener;

    [Header("State")]
    public bool IsPointerDown;

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        direction = 1;
        onPointerDown = 0.25f;

        IsPointerDown = true;
        OnPointerDownListener?.Invoke(IsPointerDown);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);

        direction = -1;
        onPointerDown = 0.4f;
        
        IsPointerDown = false;
        OnPointerDownListener?.Invoke(IsPointerDown);
    }

    private void Update()
    {
        /*var speed = direction > 0 ? UpSpeed : DowndSpeed;
        verticalValue += direction * speed * Time.deltaTime;
        verticalValue = Mathf.Clamp(verticalValue, VerticalMinValue, VerticalMaxValue);*/

        var horizontal = base.Vertical > 0 ? base.Horizontal : 0;

        verticalValue = (Mathf.Clamp01(base.Vertical + onPointerDown) + Mathf.Abs(horizontal) * 0.5f) * 
                        (VerticalMaxValue - VerticalMinValue) + VerticalMinValue;
        
        // Debug.Log(verticalValue);
    }
}