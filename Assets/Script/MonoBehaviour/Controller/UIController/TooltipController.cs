using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TooltipController : MonoBehaviour
{
    [SerializeField]
    private float keepTime;
    [SerializeField]
    private float fadeRate;

    private float currentFade;
    private Timer timer;
    private Text text;

    public void SetTooltip(string text)
    {
        timer.NextTime();
        this.text.text = text;
        currentFade = 1;
        this.text.color = Color.white;
        gameObject.SetActive(true);
    }

    private void Awake()
    {
        timer = new Timer(keepTime);
        text = GetComponent<Text>();
        gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (currentFade <= 0)
        {
            gameObject.SetActive(false);
        }
        else if(timer.ReachedTime())
        {
            currentFade -= fadeRate;
            text.color = new Color(text.color.r, text.color.g, text.color.b, currentFade);
        }
    }
}
