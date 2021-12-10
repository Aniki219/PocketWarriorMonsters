using HelperFunctions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This class is intended to be used like an Enum. It cant be an enum
 * because we've attached all of the super effective moves etc..
 * Instead it compiles all of the PokemonTypes into a static list
 * which can be searched using the static `get` method.
 * We also have getters to determine this type's effectiveness 
 * against another. These allow you to input either the PokemonType
 * or the TypeEnum. Feels messy but let's just not touch it and see
 * what happens.
 * */

[CreateAssetMenu(menuName = "Pokemon Data/Type")]
public class PokemonType : ScriptableObject
{
    [SerializeField][SearchableEnum] private TypeEnum typeEnum;
    [SerializeField] private List<PokemonType> superEffectiveAgainst;
    [SerializeField] private List<PokemonType> notVeryEffectiveAgainst;
    [SerializeField] private List<PokemonType> doesNotEffectList;

    private static Dictionary<TypeEnum, PokemonType> typeList =
        new Dictionary<TypeEnum, PokemonType>();

    public TypeEnum getTypeEnum()
    {
        return typeEnum;
    }

    public List<PokemonType> getSuperEffective()
    {
        return superEffectiveAgainst;
    }

    public List<PokemonType> getNotVeryEffective()
    {
        return notVeryEffectiveAgainst;
    }

    public List<PokemonType> getNotEffective()
    {
        return doesNotEffectList;
    }

    public bool isSuperEffective(TypeEnum againstType)
    {
        return superEffectiveAgainst.Contains(get(againstType));
    }

    public bool isNotEffective(TypeEnum againstType)
    {
        return notVeryEffectiveAgainst.Contains(get(againstType));
    }

    public bool doesNotEffect(TypeEnum againstType)
    {
        return doesNotEffectList.Contains(get(againstType));
    }

    public bool isSuperEffective(PokemonType againstType)
    {
        return superEffectiveAgainst.Contains(againstType);
    }

    public bool isNotEffective(PokemonType againstType)
    {
        return notVeryEffectiveAgainst.Contains(againstType);
    }

    public bool doesNotEffect(PokemonType againstType)
    {
        return doesNotEffectList.Contains(againstType);
    }

    /* These static methods allow us to use PokemonTypes almost like
     * an enum. These types are scriptable objects and exist as
     * instances inside of Resources/InternalData.
     * We can use the get() method to get a specific type using the 
     * TypeEnum. 
     * PokemonType.get(TypeEnun.FIRE)
     */
    [RuntimeInitializeOnLoadMethod]
    private static void loadTypesIntoMap() {
        PokemonType[] typesData =
            Resources.LoadAll<PokemonType>("InternalData/Pokemon Types");
        foreach (PokemonType t in typesData)
        {
            typeList.Add(t.getTypeEnum(), t);
        }
    }

    public static PokemonType get(TypeEnum type)
    {
        PokemonType returnType;
        if (typeList.TryGetValue(type, out returnType))
        {
            return typeList[type];
        } else
        {
            throw new Exception("Did not find " +
                "PokemonType entry for " + type.ToString());
        }
    }

    public static PokemonType get(string typeName)
    {
        TypeEnum typeEnum = EnumHelper.GetEnum<TypeEnum>(typeName);
        return get(typeEnum);
    }
}

public enum TypeEnum
{
    NORMAL,
    BUG,
    ELECTRIC,
    POISON,
    FIRE,
    DRAGON,
    FIGHTING,
    GHOST,
    PSYCHIC,
    GROUND,
    WATER,
    DARK,
    FLYING,
    STEEL,
    ICE,
    ROCK,
    GRASS,
    FAIRY,
    NONE
}
