using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/* The BattlePlan is actually made up of planMoves. These show who is set to act,
 * what move they will use, and which targets they are attacking.
 * The only thing this controller needs to do is slide in and out and provide the
 * list of planMoves soo their data is accessible
 * */
public class BattlePlanController : MonoBehaviour
{
    public List<PlanMoveController> planMoves;
    public List<PlanMoveController> previousPlanMoves;
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

    /* Delete all battle plan info
     * We may want to come back to this so that we can
     * set default targets and moves based on what was
     * used last turn.
     * */
    public void Clear()
    {
        previousPlanMoves.Clear();
        foreach (PlanMoveController plan in planMoves)
        {
            //previousPlanMoves.Add(new PlanMoveController());
            plan.Reset();
        }
    }
}

