using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowThenDie : MonoBehaviour
{
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim= GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.normalizedTime >= 1.0f)
        {
            Destroy(gameObject);
        }
        
    }
}
