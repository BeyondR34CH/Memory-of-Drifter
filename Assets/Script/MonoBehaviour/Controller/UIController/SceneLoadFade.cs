using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneLoadFade : MonoBehaviour
{
    [SerializeField]
    private float keepTime;
    [SerializeField]
    private float fadeRate;

    private float currentRate;
    private float currentFade;
    private float targetFade;
    private Timer timer;
    private Image image;

    public void EnableFade(float start, float target)
    {
        timer.NextTime();
        currentRate = start - target < 0 ? fadeRate : -fadeRate;
        currentFade = start;
        targetFade = target;
        image.color = new Color(0, 0, 0, currentFade);
        gameObject.SetActive(true);
    }

    private void Awake()
    {
        timer = new Timer(keepTime);
        image = GetComponent<Image>();
        EnableFade(1, 0);

        GameManager.OnTransScene += () => EnableFade(1, 0);
        GameManager.OnGameOver += () => EnableFade(0, 1);
    }

    private void FixedUpdate()
    {
        if (currentRate < 0 ? currentFade <= targetFade : currentFade >= targetFade)
        {
            GameManager.instance.player.GetComponent<Collider2D>().enabled = true;
            gameObject.SetActive(false);
        }
        else if (timer.ReachedTime())
        {
            currentFade += currentRate;
            image.color = new Color(0, 0, 0, currentFade);
        }
    }
}
