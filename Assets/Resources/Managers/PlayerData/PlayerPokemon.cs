using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This is the list of the player's pokemon data
 */
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

    //This should eventually be moved. This initialized a pokemon's stats.
    public void setPokemonStats()
    {
        for (int i = 0; i < pokemon.Count; i++)
        {
            Pokemon poke = pokemon[i];
            PokemonName randomPokemonName = (PokemonName)(Random.Range(1, 490));
            PokemonData data = PokedexDataReader.getPokemonData(randomPokemonName);
            int level = 50;//Random.Range(1, 100);
            int xp = Random.Range(0, 100);
            pokemon[i] = Pokemon.fromData(data, level, xp);
        }
    }
}
