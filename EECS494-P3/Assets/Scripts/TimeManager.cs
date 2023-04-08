using UnityEngine;

public static class TimeManager {
    private const float defaultFixedDT = 1f / 60;

    public static void ResetTimeScale() {
        SetTimeScale(1f);
    }

    //Expects a value in the range (0, 1]
    public static void SetTimeScale(float scale) {
        Time.timeScale = scale;
        scale = scale switch {
            // Prevent division by 0
            <= 0.01f => 0.01f,
            _ => scale
        };
        Time.fixedDeltaTime = defaultFixedDT * (1 / scale);
    }
}