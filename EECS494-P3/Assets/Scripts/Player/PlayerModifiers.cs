using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class PlayerModifiers
{
    public static float moveSpeed = 1;
    public static float damage = 1;
    public static float reloadSpeed = 1;
    public static float maxPierce = 1;
    public static float explosiveRadius = 1;

    public static void resetModifiers()
    {
        moveSpeed = 1;
        damage = 1;
        reloadSpeed = 1;
        maxPierce = 1;
        explosiveRadius = 1;
    }
}
