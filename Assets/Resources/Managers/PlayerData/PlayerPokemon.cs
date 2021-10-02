using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PokemonData")]
public class PlayerPokemon : ScriptableObject
{
    public List<Pokemon> pokemon;
    public static PlayerPokemon instance;

    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        instance = Resources.LoadAll<PlayerPokemon>("Managers/PlayerData")[0];
    }
}
