using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlanMoveController : MonoBehaviour
{
    [SerializeField] private Image pokemonIcon;
    [SerializeField] private Image moveIcon;
    [SerializeField] private Text moveText;
    [SerializeField] private List<Image> targetIcons;
    [SerializeField] private Color highlightColor = Color.white;
    [SerializeField] private Color fadeColor = Color.grey;
    [SerializeField] private int num;

    Sprite[] pokeIconSprites;


    // Start is called before the first frame update
    void Start()
    {
        Reset();
    }

    // Update is called once per frame
    void Update()
    {
        if (pokeIconSprites != null)
        {
            int i = BattleController.tick;
            pokemonIcon.sprite = pokeIconSprites[i];
        }

        if (num == BattleController.currentPokemonIndex)
        {
            moveIcon.color = new Color(moveIcon.color.r, moveIcon.color.g, moveIcon.color.b, 0.5f + 0.5f * Mathf.Sin(Time.time * 4));
        } else
        {
            moveIcon.color = Color.white;
        }
    }

    public void Reset()
    {
        pokeIconSprites = null;
        pokemonIcon.sprite = null;
        pokemonIcon.enabled = false;

        moveIcon.sprite = null;
        moveIcon.enabled = false;

        moveText.text = "";

        foreach (Image i in targetIcons)
        {
            i.enabled = false;
            i.color = fadeColor;
        }
    }

    public void setPokemon(string name)
    {
        pokemonIcon.enabled = true;
        pokeIconSprites = Resources.LoadAll<Sprite>("Sprites/Pokemon/Overworld/" + name.ToLower());
    }

    public void setMove(PokemonMove move)
    {
        moveIcon.enabled = true;
        moveIcon.sprite = Resources.LoadAll<Sprite>("Sprites/Battle/Icons/TypeIconSprites")[(int)move.getType().getTypeEnum()];
        moveText.text = move.getName();
    }

    public void setTargets(List<FieldSlotController> targets)
    {
        for (int i = 0; i < targetIcons.Count; i++)
        {
            Image t = targetIcons[i];
            if (i < 3) { t.enabled = true; }
            t.color = fadeColor;
        }
        foreach (FieldSlotController target in targets)
        {
            targetIcons[target.slotNumber].color = highlightColor;
            if (target.slotNumber >= 3)
            {
                foreach (Image t in targetIcons)
                {
                    t.enabled = true;
                }
            }
        }
    }
}
