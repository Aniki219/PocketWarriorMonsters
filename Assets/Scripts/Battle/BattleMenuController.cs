using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static MenuButtonController;

public class BattleMenuController : MonoBehaviour
{
    //delte this
    public List<FieldSlotController> enemies;
    public MoveMenuController moveMenu;
    private BattleController battleController;
    private Animator anim;
    [SerializeField] private Button[] buttons;

    private void Start()
    {
        battleController = transform.parent.GetComponentInChildren<BattleController>();
        anim = GetComponent<Animator>();
    }

    /* Bring up the BattleMenu with an animation
     * Also set the buttons to interactable and select the first one. */
    public async Task Show(int allyIndex)
    {
        anim.SetTrigger("Appear");
        foreach (Button b in buttons)
        {
            b.interactable = true;
        }
        await Task.Delay(500);
        buttons[0].Select();
    }

    /* Hide the BattleMenu with an aimation */
    public async Task Hide()
    {
        anim.SetTrigger("Disappear");
        foreach (Button b in buttons)
        {
            b.interactable = false;
        }
        await Task.Yield();
    }

    public async Task Disable()
    {
        foreach (Button b in buttons)
        {
            b.interactable = false;
        }
        await Task.Yield();
    }

    /* Here we go.
     * SelectAction first waits for a BattleMenuAction to be selected
     * these are FIGHT, POKEMON, ITEM, and RUN.
     * The player can only cancel if not on the first pokemon.
     * Cancelling will require we reselct the action for the previous pokemon.
     * Each MenuButton fires a menuButtonSelected UnityEvent<BattleMenuAction>
     * this is the enum which contains FIGHT, ITEM, etc...
     * Once we have that we can call a new async function to handle that option. 
     * We return a list of BattleActions, these can be pokemon moves, an item, running, 
     * or swapping one pokemon for another.
     */
    public async Task<List<BattleAction>> SelectActions()
    {
        List<BattleAction> battleActions = new List<BattleAction>();

        //For each player pokemon we must choose an action
        for (int i = 0; i < battleController.allyFieldSlots.Length;)
        {
            //Make battle menu visible
            await Show(i);
            //Select Action for the ith pokemon
            BattleMenuAction choice = await WaitFor.Event(MenuButtonController.menuButtonSelected);
            await Hide();
            switch (choice)
            {
                case BattleMenuAction.FIGHT:
                    BattleMove move = await SelectMove(i);
                    if (move == null)
                    {
                        //player canceled move
                        break;
                    }
                    battleActions.Add(move);
                    i++;
                    break;
                case BattleMenuAction.POKEMON:
                    i++;
                    break;
                case BattleMenuAction.ITEM:
                    i++;
                    break;
                case BattleMenuAction.RUN:
                    i++;
                    break;
                default:
                    /* Player Cancelled
                     * We must go back to the previous pokemon and remove the action it 
                     * added to the list.
                     * If there is no previous pokemon we just need to goto SelectAction */
                    if (i > 0)
                    {
                        /* If we are on pokemon 2, we go back to pokemon 1,
                         * remove pokemon 1's action, and goto SelectAction
                         * */
                        i--;
                        battleActions.RemoveAt(i);
                    }
                    break;
            }
        }
        return battleActions;
    }

    /* When the player selects FIGHT they must now choose a move and a target for
     * each pokemon on their team.
     * The player can cancel this menu by pressing 'BACK'.
     * If they did so while selecting a move, we return to the previous pokemon.
     * If it was the first pokemon, we return to SelectAction.
     * If we are selecting a target we go back to selecting a move for the current pokemon
     * Once a move and targets have been selected for each allied pokemon, we return a list
     * of BattleMoves to be added to the BattleController */
    public async Task<BattleMove> SelectMove(int allyIndex)
    {
    //goto point
    SelectMove:

        //Show move buttons
        moveMenu.Show(allyIndex);

        //Hide and reset target selection
        PokemonMove move = await WaitFor.Event(MoveButtonController.moveButtonSelected);
        if (move == null)
        {
            //player pressed cancel
            return null;
        }
        //Wait for button press animation
        await Task.Delay(200);
        moveMenu.Hide();

        List<FieldSlotController> targets = enemies; // await WaitFor.Event(targetButtonSelected);
        if (targets == null)
        {
            //player cancelled
            goto SelectMove;
        }

        
        return new BattleMove(move, targets);
    }
}

