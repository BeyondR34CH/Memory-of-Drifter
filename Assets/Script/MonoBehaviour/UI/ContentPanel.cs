using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContentPanel : BasePanel
{
    private Image header;
    private float targetFillAmount;

    private Image cellImage;

    protected override void Awake()
    {
        base.Awake();
        header = transform.GetChild(0).GetChild(1).GetComponent<Image>();

        cellImage = cell.GetComponent<Image>();
    }

    private void Update()
    {
        if (!rect.sizeDelta.Equals(targetSize))
        {
            if (!targetSize.Equals(defaultSize) && rect.sizeDelta.x >= targetSize.x * 0.95f)
            {
                targetFillAmount = 1;
                rect.sizeDelta = targetSize;
            }
            else rect.sizeDelta = NextValue(rect, targetSize);
        }
        if (!header.fillAmount.Equals(targetFillAmount))
        {
            header.fillAmount = NextValue(header, targetFillAmount);
        }
    }

    public void Open()
    {
        targetSize = openSize;
        cellImage.color = Color.white;
        isOpen = true;
    }

    public void Close()
    {
        targetSize = defaultSize;
        header.fillAmount = 0;
        targetFillAmount = 0;
        cellImage.color = Color.clear;
        isOpen = false;
    }
}
