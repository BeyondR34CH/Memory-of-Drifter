using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoldButtonController : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Button button;
    public Image hold;

    public event Action<HoldButtonController> SetCurrentButton;
    public event Action OnPerform;

    [NonSerialized] public bool isHold;
    [NonSerialized] public bool isPerform;

    public void OnSelect(BaseEventData eventData) => SetCurrentButton?.Invoke(this);

    public void OnDeselect(BaseEventData eventData) => SetCurrentButton?.Invoke(null);

    public void OnPointerEnter(PointerEventData eventData) => SetCurrentButton?.Invoke(this);

    public void OnPointerExit(PointerEventData eventData) => SetCurrentButton?.Invoke(null);

    private void Awake()
    {
        isHold = false;
        isPerform = false;
    }

    private void Update()
    {
        if (isHold)
        {
            if (!isPerform)
            {
                if (hold.fillAmount > 0.95f)
                {
                    isPerform = true;
                    hold.fillAmount = 1;
                }
                else hold.fillAmount += 0.015f * (isHold ? 1 : -1);
            }
        }
        else 
        {
            if (isPerform)
            {
                OnPerform?.Invoke();
                isPerform = false;
                hold.fillAmount = 0;
            }
            else hold.fillAmount += 0.015f * (isHold ? 1 : -1);
        }
    }
}
