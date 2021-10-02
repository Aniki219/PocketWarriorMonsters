public class PokemonStat
{
    public Stats statName;
    public int amount;
    public int IVs;
    public int EVs;

    public PokemonStat(Stats statName, int amount, int ivs = 0, int evs = 0)
    {
        this.statName = statName;
        this.amount = amount;
        IVs = ivs;
        EVs = evs;
    }
}

public class PokemonStatBuff
{
    public Stats statName;
    public int level;
    public int multiplier;
}