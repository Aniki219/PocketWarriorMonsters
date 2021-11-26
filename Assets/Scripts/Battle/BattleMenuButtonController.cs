using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BattleMenuButtonController : MenuButtonController
{
    public enum BattleMenuAction
    {
        FIGHT,
        POKEMON,
        ITEM,
        RUN,
        CANCEL
    }
    [SerializeField] BattleMenuAction battleAction;

    public static UnityEvent<BattleMenuAction> menuButtonSelected = new UnityEvent<BattleMenuAction>();

    public override void OnSubmit(BaseEventData eventData)
    {
        base.OnSubmit(eventData);
        menuButtonSelected.Invoke(battleAction);
    }

    public override void OnCancel(BaseEventData eventData)
    {
        menuButtonSelected.Invoke(BattleMenuAction.CANCEL);
    }
}
