using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MoveMenuController : MonoBehaviour
{
    private Animator anim;
    [SerializeField] private Button[] buttons;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    /* Bring up the BattleMenu with an animation
     * Also set the buttons to interactable and select the first one. */
    public void Show()
    {
        foreach (Button b in buttons)
        {
            b.gameObject.SetActive(true);
            b.interactable = true;
        }
        buttons[0].Select();
    }

    /* Hide the BattleMenu with an aimation */
    public void Hide()
    {
        foreach (Button b in buttons)
        {
            b.gameObject.SetActive(false);
            b.interactable = false;
        }
    }
}

