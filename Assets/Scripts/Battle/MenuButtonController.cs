using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButtonController : MonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler, ICancelHandler
{
    [SerializeField] protected Color defaultTextColor = Color.black;
    [SerializeField] protected Color selectedTextColor = Color.white;

    [SerializeField] protected Text buttonText;
    [SerializeField] protected Transform menuSelector;

    protected Animator anim;
    protected Button button;

    protected virtual void Start()
    {
        if (buttonText != null) defaultTextColor = buttonText.color;
        button = GetComponent<Button>();
        anim = GetComponent<Animator>();
        setMenuSelector(false);
    }
    public virtual void OnDeselect(BaseEventData eventData)
    {
        setButtonTextColor(defaultTextColor);
        setMenuSelector(false);
    }

    public virtual void OnSelect(BaseEventData eventData)
    {
        setButtonTextColor(selectedTextColor);
        setMenuSelector(true);
    }

    public virtual void OnSubmit(BaseEventData eventData)
    {
        if (anim != null) anim.SetTrigger("Press");
    }

    public virtual void OnCancel(BaseEventData eventData)
    {
        throw new NotImplementedException();
    }

    public virtual void OnDisable()
    {
        setMenuSelector(false);
    }

    private void setMenuSelector(bool active)
    {
        if (menuSelector != null)
        {
            menuSelector.gameObject.SetActive(active);
        }
    }

    private void setButtonTextColor(Color clr)
    {
        if (buttonText != null)
        {
            buttonText.color = clr;
        }
    }
}

