using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MoveMenuController : MonoBehaviour
{
    [SerializeField] private Animator anim;
    private BattleController battleController;
    [SerializeField] private Button[] buttons;

    private void Start()
    {
        battleController = transform.parent.GetComponentInChildren<BattleController>();
    }

    /* Bring up the BattleMenu with an animation
     * Also set the buttons to interactable and select the first one. */
    public void Show(int allyIndex)
    {
        anim.SetBool("Showing", true);
        for (int i = 0; i < buttons.Length; i++)
        {
            Button b = buttons[i];
            b.gameObject.SetActive(true);
            b.interactable = true;

            MoveButtonController mbc = b.gameObject.GetComponent<MoveButtonController>();
            Pokemon source = battleController.allyFieldSlots[allyIndex].pokemon;
            PokemonMove move = source.moves[i];

            mbc.setMove(move);
        }
        buttons[0].Select();
    }

    /* Hide the BattleMenu with an aimation */
    public void Hide()
    {
        anim.SetBool("Showing", false);
        foreach (Button b in buttons)
        {
            b.gameObject.SetActive(false);
            b.interactable = false;
        }
    }

    public void Disable()
    {
        foreach (Button b in buttons)
        {
            b.interactable = false;
        }
    }
}

