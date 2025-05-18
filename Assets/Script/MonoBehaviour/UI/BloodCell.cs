using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BloodCell : MonoBehaviour
{
    private Image blood;
    private Image buffer;

    private int bloodTarget;

    private void Awake()
    {
        blood = transform.GetChild(0).Find("Blood").GetComponent<Image>();
        buffer = transform.GetChild(0).Find("Buffer").GetComponent<Image>();
    }

    private void OnEnable()
    {
        Fill();
    }

    private void Update()
    {
        blood.fillAmount = Mathf.Lerp(blood.fillAmount, bloodTarget, GameManager.setting.followSpeed);
        buffer.fillAmount = Mathf.Lerp(buffer.fillAmount, blood.fillAmount, GameManager.setting.followSpeed);
    }

    public void SetCellColor(Color color) => blood.color = color;

    public void Fill()
    {
        bloodTarget = 1;
    }

    public void Pull()
    {
        blood.fillAmount = 0;
        bloodTarget = 0;
    }
}
