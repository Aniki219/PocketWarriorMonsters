using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BattleController;

/* This BattleMove can just reference an existing Move
 * This class can be deleted no problem.
 * This class also has a list of targets and can therefore construct the
 * script
 * 
 * The fields in this script are not duplicates of those in the Move class!
 * These fields are to consider the end of calculation values, after taking into account
 * the pokemon's stats and type advantages.
 * 
 * I think the battleController will apply buffs and special conditions, so this is only
 * final is that it is what it sent to the battleController, not what the opposing pokemon
 * receives.
 * In that case it is a bit strange that we calc type advantage before the pokemon is even
 * hit. This will probably need to be moved to a future receiveDamage method.
 * */
public class BattleMove : BattleAction
{
    private const float BASECRIT = 1.5f;
    private PokemonMove move;
    private List<FieldSlotController> targets;
    private Pokemon source;

    private bool didCrit = false; //This is calculated in constructor

    private PokemonStatus status;
    private int statusChance;

    public BattleMove(PokemonMove move, List<FieldSlotController> targets)
    {
        this.move = move;
        this.targets = targets;
        this.source = move.getPokemon();

        didCrit = Random.Range(0, 1.0f) <= 6.25f * (source.getStat(Stats.CRIT_CHANCE) + 1) / 100.0f;

        battleBuffer = BattleBuffer.BATTLE_MOVE;
    }

    public override string script()
    {
        string script = string.Format("{0} used {1}.<br>", source.name, move.getName());

        foreach (FieldSlotController target in targets) {
            List<PokemonType> types = new List<PokemonType>() { target.pokemon.type_1, target.pokemon.type_2 };

            float effectiveness = calcTypeEffectiveness(move.getType(), types);

            if (effectiveness == 0)
            {
                script += "It does not effect enemy " + target.name + "!<br>";
            } else
            {
                if (didCrit)
                {
                    script += "A critical hit!<br>";
                }
                if (effectiveness > 1.0f)
                {
                    script += "It's super effective!<br> ";
                }
                if (effectiveness < 1.0f) {
                    script += "It's not very effective...<br> ";
                }
                if (target.isEnemy)
                {
                    script += "Enemy ";
                }
                script += string.Format("{0} took {1} damage.<br>", 
                    target.pokemon.displayName, calcDamage(target.pokemon, targets.Count > 1));
            }

            return script;
        }
        throw new System.Exception("A BattleMove (" + move.getName() + ") was used by " 
            + source.name + " but had no targets.");
    }

    int calcDamage(Pokemon target, bool multihit = false)
    {
        int level = source.getStat(Stats.LEVEL);
        int attack = source.getStat(Stats.ATTACK);
        int defense = target.getStat(Stats.DEFENSE);

        int power = move.getPower();

        float targetMod = multihit ? 0.75f : 1.0f; //We can experiment if we even want this
        float weatherMod = 1.0f;

        float critMod = didCrit ? 1.0f : 1.5f; //BattleMoves crit as soon as they are constructed

        PokemonType type = move.getType();
        float typeMod = calcTypeEffectiveness(type, target.getTypes());
        float STAB = source.getTypes().Contains(type) ? 1.5f : 1.0f;
        float burnMod = 1.0f; //ToDo: check if source is burned and move is physical
        float randomMod = Random.Range(0.85f, 1.0f); //Damage range

        return (int)(Mathf.Round(
            ((2 * level / 5 + 2) * power * attack / defense / 50.0f + 2) *
            targetMod * weatherMod * critMod * typeMod * STAB * burnMod * randomMod
        ));
    }

    float calcTypeEffectiveness(PokemonType attackType, List<PokemonType> targetTypes)
    {
        //BASECRIT will be rasied to the power of effectiveness up to 2 down to -2
        int effectiveness = 0;
        PokemonType type = move.getType();

        foreach (PokemonType targetType in targetTypes)
        {
            if (type.doesNotEffect(targetType))
            {
                return 0;
            }
            if (type.isSuperEffective(targetType))
            {
                effectiveness++;
                continue;
            }
            if (type.isNotEffective(targetType))
            {
                effectiveness--;
                continue;
            }
        }

        return Mathf.Pow(BASECRIT, effectiveness);
    }

    public override string ToString()
    {
        return "BattleMove: " + move.getName();
    }
}
