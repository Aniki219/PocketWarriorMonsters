using System;

[Serializable]
public class PokemonStat
{
    public Stats statName;
    public int amount;
    public int IVs;
    public int EVs;

    public PokemonStat(Stats statName, int amount, int ivs = -1, int evs = 0)
    {
        this.statName = statName;
        this.amount = amount;
        IVs = ivs;
        if (IVs == -1 && !statName.Equals(Stats.CRIT_CHANCE) && !statName.Equals(Stats.ACCURACY))
        {
            IVs = UnityEngine.Random.Range(0, 31);
        }
        EVs = evs;
    }
}

public class PokemonStatBuff
{
    public Stats statName;
    public int level;
    public int multiplier;
}