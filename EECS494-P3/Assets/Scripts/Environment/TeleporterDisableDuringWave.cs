using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IsTeleporter))]
public class TeleporterDisableDuringWave : MonoBehaviour
{
    IsTeleporter tp;
    bool activated = false;

    private void Start()
    {
        tp = GetComponent<IsTeleporter>();

    }

    // Update is called once per frame
    void Update()
    {
        if (!activated && GameControl.NightEnding)
        {
            activated = true;
            tp.Active = true;
        }
        else
        {
            tp.Active = false;
        }
    }
}
