using System.Globalization;
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

    public static PokemonNature get(Natures nature)
    {
        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        string natureName = textInfo.ToTitleCase(nature.ToString().ToLower());
        PokemonNature natureObject = Resources.Load<PokemonNature>("InternalData/Pokemon Natures/" + natureName);
        if (natureObject != null)
        {
            return natureObject;
        } else
        {
            throw new System.Exception("No nature object found of type: " + natureName);
        }
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
