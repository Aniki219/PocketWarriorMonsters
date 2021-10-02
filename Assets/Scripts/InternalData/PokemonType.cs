using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Pokemon Data/Type")]
public class PokemonType : ScriptableObject
{
    public string typeName;
    public List<PokemonType> superEffectiveAgainst;
    public List<PokemonType> notVeryEffectiveAgainst;
    public List<PokemonType> doesNotEffect;
}
