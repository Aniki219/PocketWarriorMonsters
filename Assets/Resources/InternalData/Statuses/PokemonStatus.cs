using System;
using System.Threading.Tasks;

/* A duration of -1 makes this a non-durational status like paralysis.
 * Secondary statuses can be applied ontop of other statuses. You can be
 * confused and paralysed, but you can't be asleep and poisoned.
 * */
public abstract class PokemonStatus : IComparable<PokemonStatus>
{
    protected BattleController.BattleBuffer buffer;
    public int duration;
    public Pokemon pokemon;
    public bool secondary;

    protected StatusType priority = StatusType.NONE;

    public PokemonStatus(Pokemon pokemon)
    {
        this.pokemon = pokemon;
    }

    /* I think the best way to do this is to have the BattleController pass in
     * a reference to the BattleMessageController. I don't like it, but this is
     * only going to ever work one way, so it's fine to have this dependency
     * */
    public virtual async Task<bool> DoStatus(BattleMessageController messageController)
    {
        if (duration == 0) return false;
        duration--;
        await Task.Yield();
        return true;
    }

    public int CompareTo(PokemonStatus other)
    {
        return priority.CompareTo(other.priority);
    }
}

public enum StatusType
{
    SLEEP,
    FROZEN,
    UNCONTROLLABLE,
    CONFUSION,
    ATTRACT,
    PARALYSIS,
    NONE
}
