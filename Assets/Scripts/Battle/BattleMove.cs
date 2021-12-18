using StatusEffects;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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

    //This is calculated in constructor
    private bool didCrit = false; 
    private float randomMod = 1.0f;

    private Dictionary<FieldSlotController, int> targetDamages;

    public BattleMove(PokemonMove move, List<FieldSlotController> targets)
    {
        this.move = move;
        this.targets = targets;
        source = move.getPokemon();

        setTargetDamages();

        battleBuffer = BattleBuffer.BATTLE_MOVE;
    }

    public void setTargetDamages()
    {
        if (targets == null) return;
        targetDamages = new Dictionary<FieldSlotController, int>();
        didCrit = Random.Range(0, 1.0f) <= 6.25f * (source.getBaseStat(Stats.CRIT_CHANCE) + 1) / 100.0f;
        randomMod = Random.Range(0.85f, 1.0f); //Damage range
        foreach (FieldSlotController target in targets)
        {
            targetDamages.Add(target, calcDamage(target.pokemon, targets.Count > 1));
        }
    }

    public int getDamage(FieldSlotController target)
    {
        if (targetDamages.ContainsKey(target))
        {
            return targetDamages[target];
        } else
        {
            throw new System.Exception("Target: " + target.pokemon.name + " was not damaged by" +
                " move: " + move.getName());
        }
    }

    public PokemonMove getMove()
    {
        return move;
    }

    public List<FieldSlotController> getTargets()
    {
        return targets;
    }

    public Pokemon getPokemon()
    {
        return source;
    }

    public override List<string> script()
    {
        List<string> scriptLines = new List<string>();
        if (targets.Count == 0)
        {
            throw new System.Exception("A BattleMove (" + move.getName() + ") was used by "
                + source.displayName + " but had no targets.");
        }
        if (move.getScript() != null) return move.getScript();

        scriptLines.Add(string.Format("<?zoom|{2}>{0} used {1}.<br>", source.name, move.getName(), source.fieldSlot.slotNumber));

        foreach (FieldSlotController target in targets) {
            string script = string.Format("<?zoom|{0}>", target.slotNumber);
            List <PokemonType> types = new List<PokemonType>() { target.pokemon.type_1, target.pokemon.type_2 };
            setTargetDamages();
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
                    target.pokemon.displayName, getDamage(target));
            }
            scriptLines.Add(script);
        }
        return scriptLines;
    }

    public void fixTargets()
    {
        //Fix targets
        List<FieldSlotController> fixedTargets = new List<FieldSlotController>();
        //For each intended target...
        foreach (FieldSlotController target in targets)
        {
            //If it is available we keep it (add to list of fixedTargets)
            if (target.isAvailable())
            {
                fixedTargets.Add(target);
                continue;
            }
            else
            //If the target is not available (e.g. fainted)
            {
                //Then if this is an AoE ability, exclude the target
                if (targets.Count > 1)
                {
                    continue;
                }
                else
                //But if it was a single target we must reroute it
                {
                    //So we get all fieldControllers of either enemies or allies
                    List<FieldSlotController> availableTargets = target.isEnemy ? 
                        battleController.enemyFieldSlots : battleController.allyFieldSlots;
                    //Filter out all of the ones which are available
                    availableTargets = availableTargets.FindAll(t => t.isAvailable());
                    //And pick a random new target from that list
                    FieldSlotController nextTarget = availableTargets[0]; //availableTargets[Random.Range(0, availableTargets.Count - 1)];
                                                                          //Then we add it to the fixed list
                    fixedTargets.Add(nextTarget);
                }
            }
        }
        //We need to recalculate the damage the move deals to the new target
        setTargetDamages();
    }

    public async Task playSound()
    {
        AudioClip clip = Resources.Load<AudioClip>("Sounds/BattleMoves/" + move.getName());
        if (clip == null)
        {
            clip = Resources.Load<AudioClip>("Sounds/BattleMoves/Tackle");
        }
        battleController.audio.PlayOneShot(clip);
        await Task.Delay((int)(clip.length * 500));
    }

    int calcDamage(Pokemon target, bool multihit = false)
    {
        int level = source.level;
        int attack = move.getCategory().Equals(MoveCategory.PHYSICAL) ?
            source.getStatValue(Stats.ATTACK) : source.getStatValue(Stats.SP_ATTACK);
        int defense = move.getCategory().Equals(MoveCategory.PHYSICAL) ? 
            target.getStatValue(Stats.DEFENSE) : target.getStatValue(Stats.SP_DEFENSE);

        int power = move.getPower();

        float targetMod = multihit ? 0.75f : 1.0f; //We can experiment if we even want this
        float weatherMod = 1.0f;

        //didCrit is calculated in constructor
        float critMod = didCrit ? 1.5f : 1.0f;

        PokemonType type = move.getType();
        float typeMod = calcTypeEffectiveness(type, target.getTypes());
        float STAB = source.getTypes().Contains(type) ? 1.5f : 1.0f;
        float burnMod = (move.getCategory().Equals(MoveCategory.PHYSICAL) && 
            source.hasStatus<Burn>()) ?  0.5f : 1.0f;
        //randomMod is calculated in constructor
        
        int dmg = (int)(Mathf.Round(
            ((2 * level / 5 + 2) * power * attack / defense / 50.0f + 2) *
            targetMod * weatherMod * critMod * typeMod * STAB * burnMod * randomMod
        ));

        return dmg;
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
        return "BattleMove: " + move.getName() + " with speed: " + source.getStatValue(Stats.SPEED);
    }

    public override int CompareTo(BattleAction action)
    {
        if (!action.GetType().Equals(typeof(BattleMove))) return 0;
        BattleMove battleMove = (BattleMove)action;
        int mySpeed = source.getStatValue(Stats.SPEED);
        int otherSpeed = battleMove.getPokemon().getStatValue(Stats.SPEED);
        //This is the correct direction for descending order
        return otherSpeed.CompareTo(mySpeed);
    }
}
