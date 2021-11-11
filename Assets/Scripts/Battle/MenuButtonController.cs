using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButtonController : MonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler
{
    public Color selectedTextColor = Color.white;
    private Color defaultTextColor = Color.black;
    private Text buttonText;
    private Transform menuSelector;

    private void Start()
    {
        buttonText = GetComponentInChildren<Text>();
        defaultTextColor = buttonText.color;
        menuSelector = transform.Find("MenuSelector");
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
        GetComponent<Animation>().Play();
    }
}
