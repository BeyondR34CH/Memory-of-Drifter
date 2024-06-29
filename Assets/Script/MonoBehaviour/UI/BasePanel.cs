using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(FollowTarget))]
public abstract class BasePanel : MonoBehaviour
{
    public BloodCell cell;
    public Vector2 openSize;
    public bool isOpen { get; protected set; }
    
    protected RectTransform rect;
    protected Vector2 defaultSize;
    protected Vector2 targetSize;

    protected virtual void Awake()
    {
        rect = GetComponent<RectTransform>();
        defaultSize = rect.sizeDelta;
        targetSize = defaultSize;
    }

    protected float NextValue(Image image, float target)
    {
        return Mathf.Lerp(image.fillAmount, target, GameManager.setting.ronversionRate * 0.25f);
    }

    protected Vector2 NextValue(RectTransform rect, Vector2 target)
    {
        return new Vector2(Mathf.Lerp(rect.sizeDelta.x, target.x, GameManager.setting.ronversionRate),
                           Mathf.Lerp(rect.sizeDelta.y, target.y, GameManager.setting.ronversionRate));
    }
}
