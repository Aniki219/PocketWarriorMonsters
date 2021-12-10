using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/* This menu coontains the target buttons for moves. Most moves hit a single
 * enemy and cannot hit allies, therefore we need only to SetActive the enemy
 * targeting buttons.
 * Eventually we will have to deal with multi-targetting abilities, and abilities
 * that target allies specifically or may also hit allies.
 * */
public class TargetMenuController : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private BattleController battleController;
    [SerializeField] private List<TargetButtonController> EnemyButtons;
    [SerializeField] private List<TargetButtonController> AllyButtons;
    [SerializeField] private List<TargetButtonController> AllEnemiesButton;
    [SerializeField] private List<TargetButtonController> AllAlliesButton;
    [SerializeField] private List<TargetButtonController> AllButton;

    /* Bring up the BattleMenu with an animation
     * Also set the buttons to interactable and select the first one. */
    public void Show(Targets targets)
    {
        anim.SetBool("Showing", true);

        switch (targets)
        {
            case Targets.ALL:
                setButtons(AllButton);
                setButtons(AllyButtons, true, false);
                setButtons(EnemyButtons, true, false);
                AllButton[0].getButton().Select();
                break;

            case Targets.ALLIES:
                setButtons(AllAlliesButton);
                setButtons(AllyButtons, true, false);
                AllAlliesButton[0].getButton().Select();
                break;

            case Targets.ALLY:
                setButtons(AllyButtons, true, true);
                AllyButtons.Find(b => b.getFieldSlots()[0].isAvailable()).getButton().Select();
                break;

            case Targets.ENEMY:
                setButtons(EnemyButtons, true, true);
                EnemyButtons.Find(b => b.getFieldSlots()[0].isAvailable()).getButton().Select();
                break;

            case Targets.ENEMIES:
                setButtons(AllEnemiesButton);
                setButtons(EnemyButtons, true, false);
                AllEnemiesButton[0].getButton().Select();
                break;

            default:
                throw new Exception("Targets enum type: " + targets.ToString() + " not supported!");
        }
    }

    private void setButtons(List<TargetButtonController> buttons, bool active = true, bool interact = true)
    {
        foreach (TargetButtonController b in buttons)
        {
            b.gameObject.SetActive(active);
            b.getButton().interactable = interact;
            b.submittable = (b.getFieldSlots().Count == 1 && b.getFieldSlots()[0].isAvailable());
            if (b.submittable)
            {
                b.setTargetInfo();
            }
        }
    }

    /* Hide the BattleMenu with an aimation */
    public void Hide()
    {
        anim.SetBool("Showing", false);

        List<TargetButtonController> buttons = AllButton
            .Concat(AllAlliesButton)
            .Concat(AllEnemiesButton)
            .Concat(EnemyButtons)
            .Concat(AllyButtons).ToList();

        foreach (TargetButtonController b in buttons)
        {
            if (!b.isActiveAndEnabled) continue;
            b.getButton().interactable = false;
            b.gameObject.SetActive(false);
        }
    }
}

