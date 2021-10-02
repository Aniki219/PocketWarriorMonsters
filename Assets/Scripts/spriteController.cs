using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spriteController : MonoBehaviour
{
    Animator anim;
    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void flipCycleOffset()
    {
        float cycleOffset = anim.GetFloat("CycleOffset");
        anim.SetFloat("CycleOffset", cycleOffset == 0 ? 0.5f : 0);
    }
}
