using UnityEngine;

[CreateAssetMenu(menuName = "Pokemon Data/Nature")]
public class PokemonNature : ScriptableObject
{
    public Natures nature;
    [SerializeField] private Stats plus;
    [SerializeField] private Stats minus;

    public Stats getPlus()
    {
        return plus;
    }

    public Stats getMinus()
    {
        return minus;
    }
}

public enum Natures
{
    ADAMANT,
    BASHFUL,
    BOLD,
    BRAVE,
    CALM,
    CAREFUL,
    DOCILE,
    GENTLE,
    HARDY,
    HASTY,
    IMPISH,
    JOLLY,
    LAX,
    LONELY,
    MILD,
    MODEST,
    NAIVE,
    NAUGHTY,
    QUIET,
    QUIRKY,
    RASH,
    RELAXED,
    SASSY,
    SERIOUS,
    TIMID
}
