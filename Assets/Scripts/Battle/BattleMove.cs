using System.Collections;
using System.Collections.Generic;

public class BattleMove : BattleAction
{
    public string moveName;
    public List<Pokemon> targets;
    public Pokemon source;
    public int baseDamage;
    public PokemonType type;
    public int speed;
    public int accuracy;
    public int critChance;
    public PokemonStatus status;
    public int statusChance;

    public BattleMove(string moveName,
        List<Pokemon> targets,
        Pokemon source,
        int baseDamage,
        PokemonType type,
        int speed,
        int accuracy,
        int critChance,
        PokemonStatus status,
        int statusChance) : base()
    {
        this.moveName = moveName;
        this.targets = targets;
        this.source = source;
        this.baseDamage = baseDamage;
        this.speed = speed;
        this.accuracy = accuracy;
        this.critChance = critChance;
        this.status = status;
        this.statusChance = statusChance;
    }

    public override string script()
    {
        string script = string.Format("{0} used {1}.", source.name, moveName);

        foreach (Pokemon target in targets) {
            PokemonType[] types = { target.type_1, target.type_2 };

            float effectiveness = 1.0f;
            foreach (PokemonType type in types)
            {
                if (type.doesNotEffect.Contains(type))
                {
                    effectiveness*=0;
                    break;
                }
                if (type.superEffectiveAgainst.Contains(type))
                {
                    effectiveness*=1.5f;
                    continue;
                }
                if (type.notVeryEffectiveAgainst.Contains(type))
                {
                    effectiveness/=1.5f;
                    continue;
                }
            }
            if (effectiveness == 0)
            {
                script += "It does not effect enemy " + target.name + "!";
            } else
            {
                if (effectiveness > 1)
                {
                    script += "It's super effective!";
                } else
                {
                    script += "It's not very effective...";
                }
                script += string.Format("{0} took {1} damage.", target.name, baseDamage);
            }

            return script;
        }
        throw new System.Exception("A BattleMove (" + moveName + ") was used by " 
            + source.name + " but had no targets.");
    }
}
