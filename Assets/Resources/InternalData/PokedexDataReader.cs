using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[CreateAssetMenu(menuName = "Managers/Pokedex Data Reader")]
public class PokedexDataReader : ScriptableObject
{
    private static PokedexDataReader instance;
    public static PokedexDataReader Instance { get { return instance; } }

    private static string path = "Data/Pokedex.json";
    public static Pokedex data;

    [RuntimeInitializeOnLoadMethod]
    public static void readPokemonData()
    {
        StreamReader reader = new StreamReader(path);
        string json = reader.ReadToEnd();
        reader.Close();
        data = JsonUtility.FromJson<Pokedex>(json);
    }

    public static List<PokemonData> getPokemonList()
    {
        return data.pokemon;
    }

    public static PokemonData getPokemonData(PokemonName pokemonName)
    {
        return data.pokemon[(int)pokemonName];
    }
}

[Serializable]
public class Pokedex
{
    public List<PokemonData> pokemon;

    public void printPokemonList()
    {
        foreach (PokemonData poke in pokemon)
        {
            Debug.Log(poke);
        }
    }
}

[Serializable]
public struct PokemonData
{
    public string name;
    public int hp;
    public int attack;
    public int defense;
    public int spattack;
    public int spdefense;
    public int speed;
    public string type1;
    public string type2;
    public int catchrate;
    public string growthrate;
}