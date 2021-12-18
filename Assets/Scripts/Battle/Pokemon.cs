using HelperFunctions;
using StatusEffects;
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

    [SerializeField] List<PokemonStat> stats;
    public List<PokemonMove> moves;

    public PokemonNature nature;

    public int current_hp;

    public int level;
    public int xp;

    public PokemonType type_1;
    public PokemonType type_2;

    public List<PokemonStatus> statuses;

    public FieldSlotController fieldSlot;

    public Pokemon(PokemonName name, PokemonType type1, PokemonType type2, int hp, int attack,
        int defense, int sp_attack, int sp_defense, int speed, int level, int xp)
    {
        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        this.name = name;
        displayName = textInfo.ToTitleCase(name.ToString().ToLower());

        this.level = level;
        this.xp = xp;

        type_1 = type1;
        type_2 = type2;

        stats = new List<PokemonStat> {
            new PokemonStat(Stats.HP, hp),
            new PokemonStat(Stats.ATTACK, attack),
            new PokemonStat(Stats.DEFENSE, defense),
            new PokemonStat(Stats.SP_ATTACK, sp_attack),
            new PokemonStat(Stats.SP_DEFENSE, sp_defense),
            new PokemonStat(Stats.SPEED, speed),
            new PokemonStat(Stats.CRIT_CHANCE, 0)
        };
        
        current_hp = getStatValue(Stats.HP);

        moves = new List<PokemonMove>();
        moves.Add(new StandardMove((Moves)UnityEngine.Random.Range(1,100), this));
        moves.Add(new StandardMove((Moves)UnityEngine.Random.Range(1, 100), this));
        moves.Add(new StandardMove((Moves)UnityEngine.Random.Range(1, 100), this));
        moves.Add(new StandardMove((Moves)UnityEngine.Random.Range(1, 100), this));

        statuses = new List<PokemonStatus>();

        nature = PokemonNature.get(EnumHelper.GetRandom<Natures>());
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
            pokemon.getBaseStat(Stats.HP),
            pokemon.getBaseStat(Stats.ATTACK),
            pokemon.getBaseStat(Stats.DEFENSE),
            pokemon.getBaseStat(Stats.SP_ATTACK),
            pokemon.getBaseStat(Stats.SP_DEFENSE),
            pokemon.getBaseStat(Stats.SPEED),
            pokemon.level,
            pokemon.xp);
    }

    public int getBaseStat(Stats stat)
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

        int common = (int)Mathf.Floor((2 * stat.amount + stat.IVs + Mathf.Floor(stat.EVs / 4.0f)) * level / 100.0f);

        if (stat.statName.Equals(Stats.HP))
        {
            return common + level + 10;
        } else
        {
            //TODO: Calculate Natures boost.
            float natureBoost = 1.0f;
            if (nature.getPlus().Equals(stat.statName))
            {
                natureBoost += 0.1f;
            }
            if (nature.getMinus().Equals(stat.statName))
            {
                natureBoost -= 0.1f;
            }
            float statusMod = (stat.statName.Equals(Stats.SPEED) && hasStatus<Paralysis>()) ? 0.5f : 1.0f;
            return Mathf.RoundToInt((common + 5) * natureBoost * statusMod);
        }
    }

    //public override string ToString()
    //{
    //    return getStat(Stats.ATTACK).ToString();
    //}

    /* A isVolatile status is one like confusion, where a pokemon can be confused
     * and poisoned.
     * A non-isVolatile or primary status would be like poison and paralysis where 
     * a pokemon cannot be aflicted by both.
     * */
    public bool addStatus(PokemonStatus status)
    {
        status.setPokemon(this);

        if (status.isVolatile)
        {
            if (statuses.Find(s => s.GetType().Equals(status.GetType())) == null)
            {
                statuses.Add(status);
                return true;
            }
        } else
        {
            if (statuses.Find(s => !s.isVolatile) == null)
            {
                Sprite statusBadge = status.getStatusBadge();
                if (statusBadge != null && fieldSlot != null)
                {
                    fieldSlot.healthbar.setStatusBadge(statusBadge);
                }
                statuses.Add(status);
                return true;
            }
        }
        return false;
    }

    //Remove all status effects with depleted duration
    public void clearStatuses()
    {
        statuses = statuses.FindAll(s => s.duration != 0);
        if (fieldSlot != null)
        {
            fieldSlot.healthbar.setStatusBadge(null);
        }
    }

    public void removeStatus(PokemonStatus status)
    {
        statuses.Remove(status);
        if (fieldSlot != null && !status.isVolatile)
        {
            fieldSlot.healthbar.setStatusBadge(null);
        }
    }

    public bool hasStatus<T>()
    {
        return statuses.FindAll(s => s.GetType().Equals(typeof(T))).Count > 0;
    }

    public bool hasNature(Natures nature)
    {
        return this.nature.Equals(nature);
    }

    public List<PokemonType> getTypes()
    {
        return new List<PokemonType> { type_1, type_2 };
    }

    public bool isFainted()
    {
        return current_hp <= 0;
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
                Sprite[] sprites = Resources.LoadAll<Sprite>(overworldPath + nameEnum.ToString().ToLower());
                if (sprites == null || sprites.Length < 2)
                {
                    sprites = Resources.LoadAll<Sprite>(overworldPath + "nidoqueen");
                }
                return sprites;
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