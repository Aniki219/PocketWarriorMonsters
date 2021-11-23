using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButtonController : MonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler
{
    public Color selectedTextColor = Color.white;
    private Color defaultTextColor = Color.black;
    private Text buttonText;
    private Transform menuSelector;
    private Animator anim;
    private Button button;

    public enum BattleMenuAction
    {
        FIGHT,
        POKEMON,
        ITEM,
        RUN
    }
    [SerializeField] BattleMenuAction battleAction;

    public static UnityEvent<BattleMenuAction> menuButtonSelected = new UnityEvent<BattleMenuAction>();

    private void Start()
    {
        button = GetComponent<Button>();
        anim = GetComponent<Animator>();
        buttonText = GetComponentInChildren<Text>();
        defaultTextColor = buttonText.color;
        menuSelector = transform.parent.Find("MenuSelector");
        menuSelector.gameObject.SetActive(false);
    }

    public void OnSelect(BaseEventData eventData)
    {
        menuSelector.gameObject.SetActive(true);
        buttonText.color = selectedTextColor;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        buttonText.color = defaultTextColor;
        menuSelector.gameObject.SetActive(false);
    }

    public void OnSubmit(BaseEventData eventData)
    {
        anim.SetTrigger("Press");
        menuButtonSelected.Invoke(battleAction);
    }

    private void OnDisable()
    {
        menuSelector.gameObject.SetActive(false);
    }
}
