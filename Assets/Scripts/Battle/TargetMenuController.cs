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
    [SerializeField] private List<Button> buttons;

    /* Bring up the BattleMenu with an animation
     * Also set the buttons to interactable and select the first one. */
    public void Show()
    {
        anim.SetBool("Showing", true);
        for (int i = 0; i < buttons.Count; i++)
        {
            Button b = buttons[i];

            //Don't enable buttons for defeated pokemon
            List<FieldSlotController> fieldSlots = new List<FieldSlotController>();
            fieldSlots.AddRange(battleController.enemyFieldSlots);
            fieldSlots.AddRange(battleController.allyFieldSlots);
            if (!fieldSlots[i].isAvailable())
            {
                buttons[i].gameObject.SetActive(false);
                continue;
            }

            //TODO: Enable the actual available targets
            //Will probably also need to set up button navigation
            if (i < 3) { buttons[i].gameObject.SetActive(true); }
            b.GetComponent<TargetButtonController>().setTargetInfo();
        }
        //Find the first enabled button and select it
        buttons.Find(b => b.IsActive()).Select();
    }

    /* Hide the BattleMenu with an aimation */
    public void Hide()
    {
        anim.SetBool("Showing", false);
        foreach (Button b in buttons)
        {
            b.gameObject.SetActive(false);
        }
    }
}

