using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TargetButtonController : MenuButtonController
{
    [SerializeField] private FieldSlotController fieldSlot;

    [SerializeField] private Text pokeName;
    [SerializeField] private Text pokeLevel;
    [SerializeField] private Image pokeSprite;
    private Sprite[] pokeSprites;
    [SerializeField] private Image type1Icon;
    [SerializeField] private Image type2Icon;

    private bool submitted = false;

    [SerializeField] private BattlePlanController battlePlan;

    public static UnityEvent<bool> targetButtonSelected = new UnityEvent<bool>();

    public void setTargetInfo()
    {
        if (fieldSlot.pokemon == null) return;

        Pokemon pokemon = fieldSlot.pokemon;
        pokeName.text = pokemon.displayName;
        pokeLevel.text = "Lv. " + pokemon.level.ToString();
        pokeSprites = pokemon.getOverworldSpritesheet();

        Sprite[] typeIcons = Resources.LoadAll<Sprite>("Sprites/Battle/Icons/TypeIconsWithText");
        List<PokemonType> types =  fieldSlot.pokemon.getTypes();

        if (types.Count != 2) throw new Exception(pokemon.displayName + " does not have two types!");
        if (types[0].getTypeEnum().Equals(TypeEnum.NONE)) throw new Exception(pokemon.displayName + " has NONE as type_1!");
        type1Icon.sprite = typeIcons[(int)fieldSlot.pokemon.getTypes()[0].getTypeEnum()];
        if (types[1].getTypeEnum().Equals(TypeEnum.NONE))
        {
            type2Icon.enabled = false;
        }
        else
        {
            type2Icon.enabled = true;
            type2Icon.sprite = typeIcons[(int)fieldSlot.pokemon.getTypes()[1].getTypeEnum()];
        }
    }

    private void Update()
    {
        //This is too make the sprite Icons walk
        if (pokeSprites != null && pokeSprites.Length > 0)
        {
            pokeSprite.sprite = pokeSprites[BattleController.tick];
        }
    }

    /* Because a move may hit multiple targets, we need to start constructing
     * a list of targets. This may actually change now that we know we cannot
     * select multiple buttons as was the plan here.
     * For now, when we hover a button we add the target to a list of targets.
     * Unhovering removes this target.
     * The list of targets is stored in the planMove because we are dealing with
     * the list of targets for a specific move.
     * */
    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        submitted = false;
        Camera.main.GetComponent<CameraController>().SetTarget(fieldSlot.transform.position);
        battlePlan.planMoves[BattleController.currentPokemonIndex].addTarget(fieldSlot);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        if (submitted == false)
        {
            battlePlan.planMoves[BattleController.currentPokemonIndex].removeTarget(fieldSlot);
        }
    }

    /* This event sends a true or false to signify if a target was actually selected
     * or if the event was cancelled.
     * This event is picked up in the BattleMenuController.
     * */
    public override void OnSubmit(BaseEventData eventData)
    {
        targetButtonSelected.Invoke(true);
        submitted = true;
    }

    public override void OnCancel(BaseEventData eventData)
    {
        targetButtonSelected.Invoke(false);
    }

    
}
