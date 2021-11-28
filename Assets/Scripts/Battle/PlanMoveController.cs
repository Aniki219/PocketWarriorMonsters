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

    public List<FieldSlotController> targets;

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

        targets.Clear();
        setTargets();
    }

    public void setPokemonIcon(string name)
    {
        pokemonIcon.enabled = true;
        pokeIconSprites = Pokemon.getOverworldSpritesheet(name);
    }

    public void setMove(PokemonMove move)
    {
        moveIcon.enabled = true;
        moveIcon.sprite = Resources.LoadAll<Sprite>("Sprites/Battle/Icons/TypeIconSprites")[(int)move.getType().getTypeEnum()];
        moveText.text = move.getName();
    }

    public void setTargets()
    {
        for (int i = 0; i < 6; i++)
        {
            targetIcons[i].color = fadeColor;
            if (i >= 3) { 
                targetIcons[i].enabled = false;
            }
        }
        foreach (FieldSlotController fc in targets)
        {
            int i = fc.slotNumber;
            if (!fc.isEnemy)
            {
                i += 3;
                for (int j = 3; j < 6; j++)
                {
                    targetIcons[j].enabled = true;
                }
            }
            targetIcons[i].color = highlightColor;
        }
    }

    public void addTarget(FieldSlotController target)
    {
        if (!targets.Contains(target))
        {
            targets.Add(target);
            setTargets();
        }
    }

    public void removeTarget(FieldSlotController target)
    {
        if (targets.Contains(target))
        {
            targets.Remove(target);
            setTargets();
        }
    }
}
