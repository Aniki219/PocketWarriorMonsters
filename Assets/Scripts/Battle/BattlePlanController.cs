using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BattlePlanController : MonoBehaviour
{
    public List<PlanMoveController> planMoves;
    Animator anim;
    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void Show()
    {
        anim.SetBool("Showing", true);
    }

    public void Hide()
    {
        anim.SetBool("Showing", false);
    }
}

