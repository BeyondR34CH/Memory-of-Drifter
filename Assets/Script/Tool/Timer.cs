using UnityEngine;

public class Timer
{
    private float lastTime;
    private float nextTimer;
    public float blank { get; set; }
    public float elapsed => Time.time - lastTime;

    public Timer(float blank)
    {
        lastTime = Time.time - blank;
        this.blank = blank;
        nextTimer = Time.time;
    }

    public static implicit operator float(Timer timer) => timer.nextTimer;

    public bool ReachedTime()
    {
        return Time.time >= nextTimer;
    }

    public void NextTime()
    {
        lastTime = Time.time;
        nextTimer = Time.time + blank;
    }

    public void NextTime(float extraBlank)
    {
        lastTime = Time.time;
        nextTimer = Time.time + blank + extraBlank;
    }
}
