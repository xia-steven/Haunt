using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TimeManager {
    private static float defaultFixedDT = 1f / 60;

    public static void ResetTimeScale() {
        SetTimeScale(1f);
    }

    //Expects a value in the range (0, 1]
    public static void SetTimeScale(float scale) {
        Time.timeScale = scale;
        // Prevent division by 0
        if (scale <= 0.01f) scale = 0.01f;
        Time.fixedDeltaTime = defaultFixedDT * (1 / scale);
    }
}