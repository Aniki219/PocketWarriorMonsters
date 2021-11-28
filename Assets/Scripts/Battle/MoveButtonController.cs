using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MoveButtonController : MenuButtonController
{
    [SerializeField][SearchableEnum] private Moves moveEnum;
    private PokemonMove move;

    private Text moveText;
    private Text ppText;
    private Image sprite;

    [SerializeField] private BattlePlanController battlePlan;

    public static UnityEvent<PokemonMove> moveButtonSelected = new UnityEvent<PokemonMove>();

    protected override void Start()
    {
        base.Start();
        moveText = transform.Find("MoveText").GetComponent<Text>();
        ppText = transform.Find("PPText").GetComponent<Text>();
        sprite = GetComponent<Image>();
    }

    /* We need to set this button to display the information
     * about the move it represents. If there is no move for this
     * button we can simply leave it null.
     * TODO: Make the button not appear if null
     * */
    public void setMove(PokemonMove move = null)
    {
        if (move == null) return;

        this.move = move;
        
        moveText.text = move.getName();
        setPpText();

        Sprite[] moveButtons = Resources.LoadAll<Sprite>("Sprites/Battle/UI/MoveButtonsSprites");
        sprite.sprite = moveButtons[(int)move.getType().getTypeEnum()];
    }

    /* When a button is pressed we start a coroutine that handles
     * player input through dialogue, selecting a target, and finally
     * adding that move as a BattleAction to the BattleController */
    public override void OnSubmit(BaseEventData eventData)
    {
        if (move.getCurrentPp() > 0)
        {
            move.decPp();
            setPpText();
            moveButtonSelected.Invoke(move);
            anim.SetTrigger("Press");
            GetComponentInParent<MoveMenuController>().Disable();
        } else
        {
            Debug.Log("No PP left for that move!");
        }
        moveButtonSelected.Invoke(move);
    }

    public override void OnCancel(BaseEventData eventData)
    {
        moveButtonSelected.Invoke(null);
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        battlePlan.planMoves[BattleController.currentPokemonIndex].setMove(move);
    }

    private void setPpText()
    {
        ppText.text = move.getCurrentPp() + "/" + move.getPp();
    }
}
