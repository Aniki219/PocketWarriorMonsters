using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MoveButtonController : MonoBehaviour, ISubmitHandler
{
    private BattleController battleController;
    [SerializeField][SearchableEnum] private Moves moveEnum;
    private PokemonMove move;

    private Text moveText;
    private Text ppText;
    private Image sprite;
    private Button button;

    private void Start()
    {
        battleController = GetComponentInParent<BattleController>();
        moveText = transform.Find("MoveText").GetComponent<Text>();
        ppText = transform.Find("PPText").GetComponent<Text>();
        sprite = GetComponent<Image>();
        button = GetComponent<Button>();
        move = new StandardMove(moveEnum, new Pokemon(PokemonName.ABOMASNOW, 1, 1, 1, 1, 1, 1, 1, 1));
        setMove(move);
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
        ppText.text = move.getCurrentPp() + "/" + move.getPp();

        Sprite[] moveButtons = Resources.LoadAll<Sprite>("Sprites/Battle/UI/MoveButtonsSprites");
        sprite.sprite = moveButtons[(int)move.getType().getTypeEnum()];
    }

    public void deleteme()
    {
        PokemonType.get(TypeEnum.FIRE);
    }

    /* When a button is pressed we start a coroutine that handles
     * player input through dialogue, selecting a target, and finally
     * adding that move as a BattleAction to the BattleController */
    public void OnSubmit(BaseEventData eventData)
    {
        if (move.getCurrentPp() > 0)
        {
            move.decPp();
        } else
        {
            Debug.Log("No PP left for that move!");
        }
        StartCoroutine(selectTargetsAddToQueue());
    }

    private IEnumerator selectTargetsAddToQueue() {
        List<FieldSlotController> targets = new List<FieldSlotController>();

        Debug.Log("Select target. Press 'A'.");
        targets.Add(battleController.enemyFieldSlots[1]);

        while (!InputManager.getKeyPressed("Confirm")) {
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("Target selected");

        BattleMove battleMove = new BattleMove(move, targets);
        battleController.addBattleActionToQueue(battleMove);
    }
}
