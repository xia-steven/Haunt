using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TimeManager
{
    private static float defaultFixedDT = 60;

    public static void ResetTimeScale()
    {
        SetTimeScale(1);
    }

    //Expects a value in the range (0, 1]
    public static void SetTimeScale(float scale)
    {
        if (scale <= 0.01f) scale = 0.01f;

        Time.timeScale = scale;
        Time.fixedDeltaTime = defaultFixedDT * scale;
    }
}
