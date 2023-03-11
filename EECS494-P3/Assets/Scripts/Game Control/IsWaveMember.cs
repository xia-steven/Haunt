using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsWaveMember : MonoBehaviour
{
    Wave owner;
    int id;

    public void Init(Wave newOwner, int _id)
    {
        owner = newOwner;
        id = _id;
    }

    private void OnDestroy()
    {
        owner.LoseMember(id);
    }
}
