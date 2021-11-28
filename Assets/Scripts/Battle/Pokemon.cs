using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

[Serializable]
public class Pokemon
{
    public const string overworldPath = "Sprites/Pokemon/Overworld/";
    [SearchableEnum]
    public PokemonName name;
    public string displayName;

    List<PokemonStat> stats;
    public List<PokemonMove> moves;

    public PokemonNature nature;

    public int current_hp;

    public int level;
    public int xp;

    public PokemonType type_1;
    public PokemonType type_2;

    public PokemonStatus status;

    public Pokemon(PokemonName name, PokemonType type1, PokemonType type2, int hp, int attack,
        int defense, int sp_attack, int sp_defense, int speed, int level, int xp)
    {
        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        this.name = name;
        displayName = textInfo.ToTitleCase(name.ToString().ToLower());

        type_1 = type1;
        type_2 = type2;

        stats = new List<PokemonStat> {
            new PokemonStat(Stats.HP, hp),
            new PokemonStat(Stats.ATTACK, attack),
            new PokemonStat(Stats.DEFENSE, defense),
            new PokemonStat(Stats.SP_ATTACK, sp_attack),
            new PokemonStat(Stats.SP_DEFENSE, sp_defense),
            new PokemonStat(Stats.SPEED, speed),
            new PokemonStat(Stats.LEVEL, level),
            new PokemonStat(Stats.CRIT_CHANCE, 0)
        };
        
        current_hp = hp;

        moves = new List<PokemonMove>();
        moves.Add(new StandardMove(Moves.ABSORB, this));
        moves.Add(new StandardMove(Moves.BLAST_BURN, this));
        moves.Add(new StandardMove(Moves.DIZZY_PUNCH, this));
        moves.Add(new StandardMove(Moves.FLAMETHROWER, this));
        
        this.level = level;
        this.xp = xp;
    }

    public static Pokemon fromData(PokemonData data, int level = 1, int xp = 0)
    {
        PokemonName name = (PokemonName)Enum.Parse(typeof(PokemonName), data.name, true);

        PokemonType type1 = PokemonType.get(data.type1);
        PokemonType type2 = PokemonType.get(data.type2);

        return new Pokemon(name,
            type1,
            type2,
            data.hp,
            data.attack,
            data.defense,
            data.spattack,
            data.spdefense,
            data.speed,
            level,
            xp);
    }

    public static Pokemon copy(Pokemon pokemon)
    {
        return new Pokemon(pokemon.name,
            pokemon.type_1,
            pokemon.type_2,
            pokemon.getStat(Stats.HP),
            pokemon.getStat(Stats.ATTACK),
            pokemon.getStat(Stats.DEFENSE),
            pokemon.getStat(Stats.SP_ATTACK),
            pokemon.getStat(Stats.SP_DEFENSE),
            pokemon.getStat(Stats.SPEED),
            pokemon.getStat(Stats.LEVEL),
            pokemon.xp);
    }

    public int getStat(Stats stat)
    {
        PokemonStat returnStat = stats.Find(s => s.statName == stat);

        if (returnStat == null)
        {
            throw new Exception("Pokemon: " + name + " does not contain a stat for " + stat);
        }

        return returnStat.amount;

    }

    public int getStatValue(Stats inStat)
    {
        PokemonStat stat = stats.Find(s => s.statName == inStat);

        if (stat == null)
        {
            throw new Exception("Pokemon: " + name + " does not contain a stat for " + stat);
        }

        int common = (int)Mathf.Floor(2 * stat.amount + stat.IVs + Mathf.Floor(stat.EVs / 4.0f) * level / 100.0f);

        if (stat.statName.Equals(Stats.HP))
        {
            return common + level + 10;
        } else
        {
            //TODO: Calculate Natures boost.
            int natureBoost = 1;

            return (common + 5) * natureBoost;
        }
    }

    //public override string ToString()
    //{
    //    return getStat(Stats.ATTACK).ToString();
    //}

    public List<PokemonType> getTypes()
    {
        return new List<PokemonType> { type_1, type_2 };
    }

    /* Here we create methods for getting the overworld sprites of a pokemon.
     * You can call it directlly on a pokemon, or statically with a pokemon name
     * enum or string.
     * This pattern should be used for all common Resource.Load calls as there is now
     * a single source containing the path string. Also it's annoying to write.
     * */
    #region Get Sprites
    public Sprite[] getOverworldSpritesheet()
    {
        return Resources.LoadAll<Sprite>(overworldPath + displayName.ToLower());
    }

    public static Sprite[] getOverworldSpritesheet(PokemonName nameEnum)
    {
        return Resources.LoadAll<Sprite>(overworldPath + nameEnum.ToString().ToLower());
    }

    public static Sprite[] getOverworldSpritesheet(string name)
    {
        name = name.ToUpper();
        PokemonName nameEnum;
        if (Enum.TryParse(name, out nameEnum))
        {
            if (Enum.IsDefined(typeof(PokemonName), nameEnum)) {
                return Resources.LoadAll<Sprite>(overworldPath + nameEnum.ToString().ToLower());
            }
            throw new Exception("PokmonName enum " + name + " is undefined!");
        } else
        {
            Debug.LogWarning("No overworld sprite for pokemon " + name);
            return Resources.LoadAll<Sprite>(overworldPath + "nidoqueen");
        }      
    }
    #endregion
}