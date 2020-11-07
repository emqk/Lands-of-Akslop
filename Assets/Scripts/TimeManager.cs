using UnityEngine;

public static class TimeManager
{
    public static void SetDefaultTime()
    {
        Debug.Log("Set default time");
        Time.timeScale = 1;
    }

    public static void SetPauseTime()
    {
        Debug.Log("Set pause time");
        Time.timeScale = 0;
    }
}
