using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    public Text endText;
    public TooltipController tooltip;

    protected override void Awake()
    {
        base.Awake();
        endText.text = "";
    }
}
