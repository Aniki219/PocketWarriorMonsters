using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Pokemon Data/Status")]
public class PokemonStatus : ScriptableObject
{
    public string type;
    public int durationMinimum;
    public int durationMaximum;
}
