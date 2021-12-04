using Febucci.UI;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static BattleMenuButtonController;

public class BattleMenuController : MonoBehaviour
{
    public MoveMenuController moveMenu;
    public TargetMenuController targetMenu;
    public BattlePlanController battlePlan;
    public TextAnimatorPlayer textPlayer;
    [SerializeField] private BattleController battleController;
    private Animator anim;
    [SerializeField] private Image currentPokemonImage;
    private Sprite[] currentPokemonImages;
    [SerializeField] private Button[] buttons;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    /* Bring up the BattleMenu with an animation
     * Also set the buttons to interactable and select the first one. */
    public async Task Show()
    {
        anim.SetBool("Showing", true);
        string pokeName = battleController.allyFieldSlots[BattleController.currentPokemonIndex].pokemon.displayName;
        textPlayer.ShowText("Select a move for " + pokeName);
        currentPokemonImages = Pokemon.getOverworldSpritesheet(pokeName);
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
        anim.SetBool("Showing", false);
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
     * Cancelling will require we reselect the action for the previous pokemon.
     * Each MenuButton fires a menuButtonSelected UnityEvent<BattleMenuAction>
     * this is the enum which contains FIGHT, ITEM, etc...
     * Once we have that we can call a new async function to handle that option. 
     * We return a list of BattleActions to the BattleController, these can be 
     * pokemon moves, an item, running, or swapping one pokemon for another.
     */
    public async Task<List<BattleAction>> SelectActions()
    {
        List<BattleAction> battleActions = new List<BattleAction>();
        /* For each player pokemon we must choose an action
         * Notice we dont  increment i automatically. We won't want to
         * do this if the player does not select an action, i.e. they cancelled
         * */
        for (int i = 0; i < battleController.allyFieldSlots.Count;)
        {
            //Skip the turn of any fieldSlots that don't have available pokemon
            if (!battleController.allyFieldSlots[i].isAvailable())
            {
                i++;
                continue;
            }
            BattleController.currentPokemonIndex = i;
            BattleController.cam.SetTarget(battleController.allyFieldSlots[i].transform.position);
            //Make battle menu visible
            await Show();
            //Select Action for the ith pokemon
            BattleMenuAction choice = await WaitFor.Event(BattleMenuButtonController.menuButtonSelected);
            await Hide();
            switch (choice)
            {
                case BattleMenuAction.FIGHT:
                    BattleMove move = await SelectMove();
                    if (move == null)
                    {
                        //player canceled move. Notice we dont increment i
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
        BattleController.cam.Reset();
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
    public async Task<BattleMove> SelectMove()
    {
        //Name shortening the index of the current pokemon (fieldSlot)
        int i = BattleController.currentPokemonIndex;
        //Storing the source of the action
        FieldSlotController source = battleController.allyFieldSlots[i];
        //Name shortening the current pokemon
        Pokemon pokemon = source.pokemon;
        //Name shortening the current planMove
        PlanMoveController planMove = battlePlan.planMoves[i];

    //For goto
    SelectMove:

        //Show the battlePlan UI
        battlePlan.Show();
        planMove.setPokemonIcon(pokemon.displayName);
        BattleController.cam.SetTarget(battleController.allyFieldSlots[i].transform.position);
        //Show move buttons
        moveMenu.Show();

        //Hide and reset target selection
        PokemonMove move = await WaitFor.Event(MoveButtonController.moveButtonSelected);
        if (move == null)
        {
            //player pressed cancel
            moveMenu.Hide();
            battlePlan.Hide();
            //We need to clear the planned move and targets for this pokemon
            battlePlan.planMoves[i].Reset();
            return null;
        }
        //Wait for move button press animation
        await Task.Delay(200);
        moveMenu.Hide();
        await Task.Delay(100);
        targetMenu.Show();

        /* Wait for player to select a target.
        * This returns a bool indicating whether a target was selected, or if
        * the player cancelled during target selection
        * */
        bool selectedTarget = await WaitFor.Event(TargetButtonController.targetButtonSelected);
        List<FieldSlotController> targets = battlePlan.planMoves[i].targets;
        if (targets.Count == 0 || !selectedTarget)
        {
            //player cancelled
            targetMenu.Hide();
            goto SelectMove;
        }
        //Wait for target button press animation
        await Task.Delay(200);
        targetMenu.Hide();
        battlePlan.Hide();

        //We return a single battle move with a list of targets to the SelectActions method
        return new BattleMove(move, targets);
    }

    private void Update()
    {
        if (currentPokemonImages != null && currentPokemonImages.Length > 1) { 
        currentPokemonImage.sprite =
            currentPokemonImages[BattleController.tick];
        }
    }
}


