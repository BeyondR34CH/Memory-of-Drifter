using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LRPanel : BasePanel
{
    private Image left;
    private Image right;

    private int direction;
    private float leftTarget;
    private float rightTarget;

    protected override void Awake()
    {
        base.Awake();
        left = transform.Find("LeftHeader").GetComponent<Image>();
        right = transform.Find("RightHeader").GetComponent<Image>();
    }

    private void Update()
    {
        if (!rect.sizeDelta.Equals(targetSize))
        {
            if (!targetSize.Equals(defaultSize) && rect.sizeDelta.x >= targetSize.x * 0.95f)
            {
                if (direction <= 0)
                {
                    leftTarget = 1;
                    rightTarget = 0;
                }
                else
                {
                    leftTarget = 0;
                    rightTarget = 1;
                }
                rect.sizeDelta = targetSize;
            }
            else rect.sizeDelta = NextValue(rect, targetSize);
        }
        if (!left.fillAmount.Equals(leftTarget))
        {
            left.fillAmount = NextValue(left, leftTarget);
        }
        if (!right.fillAmount.Equals(rightTarget))
        {
            right.fillAmount = NextValue(right, rightTarget);
        }
    }

    public void Open(int direction)
    {
        targetSize = openSize;
        this.direction = direction;
        isOpen = true;
    }

    public void SwitchLeft()
    {
        leftTarget = 1;
        rightTarget = 0;
    }

    public void SwitchRight()
    {
        leftTarget = 0;
        rightTarget = 1;
    }

    public void Close()
    {
        targetSize = defaultSize;
        left.fillAmount = 0;
        right.fillAmount = 0;
        leftTarget = 0;
        rightTarget = 0;
        isOpen = false;
    }
}
