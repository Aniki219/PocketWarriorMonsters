using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Pokemon
{
    [SearchableEnum]
    public PokemonName name;



    List<PokemonStat> stats;
    public List<PokemonMove> moves;

    public PokemonNature nature;

    public int current_hp;

    public int level;
    public int xp;

    public PokemonType type_1;
    public PokemonType type_2;

    public PokemonStatus status;

    public Pokemon(PokemonName name, int hp, int attack,
        int defense, int sp_attack, int sp_defense, int speed, int level, int xp)
    {
        this.name = name;

        stats = new List<PokemonStat> {
            new PokemonStat(Stats.HP, hp),
            new PokemonStat(Stats.ATTACK, attack),
            new PokemonStat(Stats.DEFENSE, defense),
            new PokemonStat(Stats.SP_ATTACK, sp_attack),
            new PokemonStat(Stats.SP_DEFENSE, sp_defense),
            new PokemonStat(Stats.SPEED, speed),
            new PokemonStat(Stats.LEVEL, level)
        };
        
        this.current_hp = hp;

        this.level = level;
        this.xp = xp;
    }

    public static Pokemon copy(Pokemon pokemon)
    {
        return new Pokemon(pokemon.name,
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
        //    PokemonStat returnStat = stats.Find(s => s.statName == stat);

        //    if (returnStat == null)
        //    {
        //        throw new Exception("Pokemon: " + name + " does not contain a stat for " + stat);
        //    }

        //    return returnStat.amount;
        //}
        return 999;
    }
}