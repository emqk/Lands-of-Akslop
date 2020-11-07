using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TimeController
{
    public static void Resume()
    {
        Time.timeScale = 1;
    }

    public static void Pause()
    {
        Time.timeScale = 0;
    }
}
