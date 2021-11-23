using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MoveButtonController : MonoBehaviour, ISubmitHandler, ICancelHandler, ISelectHandler, IDeselectHandler
{
    private BattleController battleController;
    [SerializeField][SearchableEnum] private Moves moveEnum;
    private PokemonMove move;

    private Text moveText;
    private Text ppText;
    private Image sprite;
    private Button button;
    private Animator anim;

    //TODO: We should really make an abstract button class...
    public Transform menuSelector;

    public static UnityEvent<PokemonMove> moveButtonSelected = new UnityEvent<PokemonMove>();

    private void Init()
    {
        battleController = GetComponentInParent<BattleController>();
        moveText = transform.Find("MoveText").GetComponent<Text>();
        ppText = transform.Find("PPText").GetComponent<Text>();
        sprite = GetComponent<Image>();
        button = GetComponent<Button>();
        anim = GetComponent<Animator>();
    }

    /* We need to set this button to display the information
     * about the move it represents. If there is no move for this
     * button we can simply leave it null.
     * TODO: Make the button not appear if null
     * */
    public void setMove(PokemonMove move = null)
    {
        Init();
        if (move == null) return;

        this.move = move;
        
        moveText.text = move.getName();
        ppText.text = move.getCurrentPp() + "/" + move.getPp();

        Sprite[] moveButtons = Resources.LoadAll<Sprite>("Sprites/Battle/UI/MoveButtonsSprites");
        Sprite[] selectedMoveButtons = moveButtons; // Resources.LoadAll<Sprite>("Sprites/Battle/UI/SelectedMoveButtonsSprites");
        sprite.sprite = moveButtons[(int)move.getType().getTypeEnum()];
        SpriteState spriteState = button.spriteState;
        spriteState.selectedSprite = selectedMoveButtons[(int)move.getType().getTypeEnum()];
        button.spriteState = spriteState;
    }

    /* When a button is pressed we start a coroutine that handles
     * player input through dialogue, selecting a target, and finally
     * adding that move as a BattleAction to the BattleController */
    public void OnSubmit(BaseEventData eventData)
    {
        if (move.getCurrentPp() > 0)
        {
            move.decPp();
            moveButtonSelected.Invoke(move);
            anim.SetTrigger("Press");
            GetComponentInParent<MoveMenuController>().Disable();
        } else
        {
            Debug.Log("No PP left for that move!");
        }
        moveButtonSelected.Invoke(move);
    }

    void ICancelHandler.OnCancel(BaseEventData eventData)
    {
        moveButtonSelected.Invoke(null);
    }

    public void OnSelect(BaseEventData eventData)
    {
        menuSelector.gameObject.SetActive(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        menuSelector.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        menuSelector.gameObject.SetActive(false);
    }
}
