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
    [SerializeField] private Button[] buttons;

    /* Bring up the BattleMenu with an animation
     * Also set the buttons to interactable and select the first one. */
    public void Show()
    {
        anim.SetBool("Showing", true);
        for (int i = 0; i < buttons.Length; i++)
        {
            Button b = buttons[i];
            b.interactable = true;
            //TODO: Enable the actual available targets
            //Will probably also need to set up button navigation
            if (i < 3) { buttons[i].enabled = true; }
            b.GetComponent<TargetButtonController>().setTargetInfo();
        }
        buttons[0].Select();
    }

    /* Hide the BattleMenu with an aimation */
    public void Hide()
    {
        anim.SetBool("Showing", false);
        foreach (Button b in buttons)
        {
            b.enabled = false;
            b.interactable = false;
        }
    }
}

